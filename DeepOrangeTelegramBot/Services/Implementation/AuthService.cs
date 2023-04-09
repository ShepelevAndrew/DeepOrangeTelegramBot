using DeepOrangeTelegramBot.Bot;
using DeepOrangeTelegramBot.Services.Interfaces;
using IdentityModel.Client;
using System.Security.Claims;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class AuthService : IAuthService
{
    public readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> GetRequestAsync(TokenResponse token, string requestUri)
    {
        var client = _httpClientFactory.CreateClient("GetRequest");

        client.SetBearerToken(token.AccessToken);

        var response = await client.GetAsync(requestUri);

        return response;
    }

    public async Task<UserInfo> GetUserInfoAsync(TokenResponse token)
    {
        var client = _httpClientFactory.CreateClient("UserInfo");

        var discover = await client.GetDiscoveryDocumentAsync("https://localhost:7004");

        var requestUserInfo = new UserInfoRequest
        {
            Address = discover.UserInfoEndpoint,
            Token = token.AccessToken
        };

        var responseUserInfo = await client.GetUserInfoAsync(requestUserInfo);
        var userInfoContent = responseUserInfo.Claims;

        if (userInfoContent is null)
            return null;

        var userInfo = new UserInfo
        {
            PreferredUsername = userInfoContent.FirstOrDefault(c => c.Type == "name").Value,
            Subject = userInfoContent.FirstOrDefault(c => c.Type == "sub").Value
        };

        return userInfo;
    }

    public string GetAuthUrl(string state)
    {
        var authUrl = "https://127.0.0.1:7004/connect/authorize?" +
                       "client_id=deep-orange-id&" +
                       "response_type=code&" +
                       "scope=openid%20profile" +
                       "%20DeepOrangeApi&" +
                       "redirect_uri=http://localhost:5000/auth&" +
                       $"state={state}";

        return authUrl;
    }

    public async Task<TokenResponse> GetTokenResponseAsync(string code)
    {
        var client = _httpClientFactory.CreateClient("tokenResponse");

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

        return response;
    }
}
