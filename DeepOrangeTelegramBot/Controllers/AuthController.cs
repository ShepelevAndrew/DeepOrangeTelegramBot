using DeepOrangeTelegramBot.Bot;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace DeepOrangeTelegramBot.Controllers
{
    [ApiController]
    [Route("/[action]")]
    public class AuthController : Controller
    {
        private HttpClient _client = Client.GetHttpClient();

        [HttpGet]
        public async Task<IActionResult> Auth(string code)
        {
            var client = new HttpClient();

            var discover = await client.GetDiscoveryDocumentAsync("https://localhost:7004");

            var request = new AuthorizationCodeTokenRequest
            {
                Address = discover.TokenEndpoint,
                ClientId = "deep-orange-id",
                ClientSecret = "deep_secret",
                GrantType = "authorization_code",
                RedirectUri = "http://localhost:5000/auth",
                Code = code
            };

            var response = await client.RequestAuthorizationCodeTokenAsync(request);

            var accessToken = response.AccessToken;
            var identityTokenn = response.IdentityToken;

            _client.SetBearerToken(accessToken);

            var requestUserInfo = new UserInfoRequest
            {
                Address = discover.UserInfoEndpoint,
                Token = accessToken
            };

            var responseUserInfo = await client.GetUserInfoAsync(requestUserInfo);
            var userInfoContent = responseUserInfo.Claims;

            return Redirect($"tg://resolve?domain=DeepOrange_bot");
        }

        [HttpGet]
        public async Task Logout()
        {
            var url = "https://localhost:7004/logout" +
                      "?logoutId=" + "" +
                      "&post_logout_redirect_uri=" + "tg://resolve?domain=DeepOrange_bot";

            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Logout successful
            }
            else
            {
                // Logout failed
            }
        }
    }
}
