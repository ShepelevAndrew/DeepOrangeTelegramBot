using DeepOrangeTelegramBot.Bot;
using DeepOrangeTelegramBot.Services.Interfaces;
using IdentityModel.Client;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class OIdcService
{
    private readonly IAuthService _authService;
    private readonly UserTrackerStorage _userTrackers;
    private readonly TokenStorage _accessTokens;

    public OIdcService(IAuthService authService, UserTrackerStorage userTrackers, TokenStorage accessTokens)
    {
        _authService = authService;
        _userTrackers = userTrackers;
        _accessTokens = accessTokens;
    }

    public async Task<UserInfo> FindUserInfoAsync(long userId)
    {
        var token = _accessTokens.Find(userId);

        if(token is null) {
            return null;
        }

        var user = await _authService.GetUserInfoAsync(token);

        return user;
    }

    public async Task<HttpResponseMessage> GetRequest(long userId, string requestUri)
    {
        var token = _accessTokens.Find(userId);

        if (token is null)
        {
            return null;
        }

        var response = await _authService.GetRequestAsync(token, requestUri);

        return response;
    }

    public string GetAuthUrl(long userId)
    {
        var state = Guid.NewGuid().ToString();
        _userTrackers.Put(state, userId);

        var authUrl = _authService.GetAuthUrl(state);

        return authUrl;
    }

    public async Task<UserInfo> CompleteAuthAsync(string state, string code)
    {
        var userId = _userTrackers.Find(state);
        var token = await RequestAndStoreTokenAsync(code, userId);

        var user = await _authService.GetUserInfoAsync(token);

        return user;
    }

    private async Task<TokenResponse> RequestAndStoreTokenAsync(string code, int userId)
    {
        var token = await RequestTokenAsync(code);
        _accessTokens.Put(userId, token);
        return token;
    }

    private async Task<TokenResponse> RequestTokenAsync(string code)
    {
        try
        {
            var token = await _authService.GetTokenResponseAsync(code);
            return token;
            
        }
        catch (Exception e) {
            throw new Exception("Cannot get access token", e);
        }
    }
}
