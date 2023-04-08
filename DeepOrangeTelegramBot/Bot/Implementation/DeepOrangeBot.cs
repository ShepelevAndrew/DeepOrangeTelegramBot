using DeepOrangeTelegramBot.Bot.Interfaces;
using Telegram.Bot;

namespace DeepOrangeTelegramBot.Bot.Implementation
{
    public class DeepOrangeBot : ITelegramBot
    {
        public TelegramBotClient Client { get; set; }

        public DeepOrangeBot(IConfiguration configuration)
        {
            var token = configuration["TelegramBotToken"];

            if (token is null)
            {
                token = "";
                Console.WriteLine("Token is null");
            }

            Client = new TelegramBotClient(token);
        }
    }
}