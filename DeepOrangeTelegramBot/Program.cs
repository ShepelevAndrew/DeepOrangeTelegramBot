using DeepOrangeTelegramBot.Bot.Implementation;
using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;

namespace DeepOrangeTelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                            .AddNewtonsoftJson();

            builder.Services.AddSingleton<UpdateDistributor<CommandExecutor>>()
                            .AddSingleton<ITelegramBot, DeepOrangeBot>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}