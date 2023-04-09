using DeepOrangeTelegramBot.Bot.Implementation;
using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using DeepOrangeTelegramBot.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DeepOrangeTelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                            .AddNewtonsoftJson();

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<UpdateDistributor>()
                            .AddSingleton<ITelegramBot, DeepOrangeBot>()
                            .AddSingleton<OIdcService>()
                            .AddSingleton<IAuthService, AuthService>()
                            .AddSingleton<UserTrackerStorage>()
                            .AddSingleton<TokenStorage>()
                            .AddSingleton<IMemoryCache, MemoryCache>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}