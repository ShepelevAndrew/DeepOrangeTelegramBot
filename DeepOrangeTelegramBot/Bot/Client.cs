using Telegram.Bot;

namespace DeepOrangeTelegramBot.Bot
{
    public class Client
    {
        private static HttpClient client { get; set; }

        public static HttpClient GetHttpClient()
        {
            if (client != null)
            {
                return client;
            }
            client = new HttpClient();
            return client;
        }
    }
}
