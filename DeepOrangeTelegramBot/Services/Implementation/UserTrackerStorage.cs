using Microsoft.Extensions.Caching.Memory;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class UserTrackerStorage
{
    public static TimeSpan expiration { get; set; } = TimeSpan.FromMinutes(30);
    private readonly MemoryCache trackers = new MemoryCache(new MemoryCacheOptions
    {
        ExpirationScanFrequency = expiration
    });

    public void Put(string tracker, long userId)
    {
        trackers.Set(tracker, userId);
    }

    public int Find(string tracker)
    {
        int userId = int.Parse(trackers.Get(tracker).ToString());

        return userId;
    }
}