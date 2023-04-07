using Telegram.Bot;

namespace DeepOrangeTelegramBot.Bot
{
    public class TelegramBot
    {
        private static TelegramBotClient client { get; set; }

        public static TelegramBotClient GetTelegramBot()
        {
            if (client != null)
            {
                return client;
            }
            client = new TelegramBotClient("2123782383:AAHJHwaPuGjSu3khAJcFleuiqR2JxsIoE-4");
            return client;
        }
    }
}