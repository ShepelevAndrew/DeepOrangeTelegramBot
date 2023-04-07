using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace DeepOrangeTelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                            .AddNewtonsoftJson();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}