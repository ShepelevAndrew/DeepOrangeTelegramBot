using DeepOrangeTelegramBot.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class TokenStorage
{
    private readonly IAuthService _authService;
    private readonly IMemoryCache _accessTokens;
    private readonly ConcurrentDictionary<long, string> _refreshTokens;

    public TokenStorage(IAuthService authService, IMemoryCache acessTokens)
    {
        _authService = authService;
        _accessTokens = acessTokens;
        _refreshTokens = new ConcurrentDictionary<long, string>();
    }

    public TokenResponse Find(long userId)
    {
        var token = _accessTokens.Get<TokenResponse>(userId);

        return token;
    }

    public void Put(long userId, TokenResponse token)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(120))
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    var token = value as TokenResponse;
                    if (reason == EvictionReason.Expired)
                    {
                        _accessTokens.Set(key, token.RefreshToken, TimeSpan.FromSeconds(token.ExpiresIn));
                    }
                });

        _accessTokens.Set(userId, token, cacheEntryOptions);
        _refreshTokens.TryAdd(userId, token.RefreshToken);
    }

    private void RefreshAccessToken(object key, object value, EvictionReason reason, object state)
    {
        int userId = (int)key;
        var accessToken = (TokenResponse)value;
        Console.WriteLine($"Cache miss (user id {userId}). Refreshing access token.");
        try
        {
            string refreshToken;
            if (!_refreshTokens.TryGetValue(userId, out refreshToken))
            {
                Console.WriteLine("Cannot find offline token. Skip refresh.");
            }
            /*accessToken = (TokenResponse)_authService.RefreshAccessToken(refreshToken);*/
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot refresh access token", e);
        }
    }
}