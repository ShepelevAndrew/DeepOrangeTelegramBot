using DeepOrangeTelegramBot.Services.Implementation;
using IdentityModel.Client;

namespace DeepOrangeTelegramBot.Services.Interfaces;

public interface IAuthService
{
    string GetAuthUrl(string state);
    Task<UserInfo> GetUserInfoAsync(TokenResponse token);
    Task<TokenResponse> GetTokenResponseAsync(string code);
    Task<HttpResponseMessage> GetRequestAsync(TokenResponse token, string requestUri);
}
