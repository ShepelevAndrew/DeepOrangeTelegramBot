using DeepOrangeTelegramBot.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DeepOrangeTelegramBot.Controllers
{
    [ApiController]
    [Route("/")]
    public class BotController : ControllerBase
    {
        public static IEnumerable<Claim> UserClaims { get; set; }
        private HttpClient client = Client.GetHttpClient();
        private TelegramBotClient bot = TelegramBot.GetTelegramBot();

        [HttpPost]
        public async void Post(Update update)
        {
            if (update.Message is null)
                return;

            long chatId = update.Message.Chat.Id;

            if(update.Message.Text == "/get")
            {

                var postResponse = await client.GetAsync("https://localhost:7156/Employee/Get");
                var content = postResponse.Content.ReadAsStringAsync();

                if (postResponse.IsSuccessStatusCode) { 
                    bot.SendTextMessageAsync(
                            chatId,
                            $"{content.Result}");
                } else
                {
                    bot.SendTextMessageAsync(
                            chatId,
                            $"{postResponse.ReasonPhrase}");
                }
            }
            else { 
                if(UserClaims is null) { 
                    await bot.SendTextMessageAsync(
                        chatId, 
                        "Будь ласка авторизуйтесь: <a href=\"https://127.0.0.1:7004/connect/authorize?client_id=deep-orange-id&response_type=code&scope=openid%20profile%20DeepOrangeApi&redirect_uri=http://localhost:5000/auth/auth&start=&state=some-state-data\">увійти</a>",
                        ParseMode.Html);
                }
                else { 
                    await bot.SendTextMessageAsync(
                        chatId,
                        $"Привіт, {UserClaims.FirstOrDefault(x => x.Type == "name").Value}");
                }
            }

            Console.WriteLine(update.Message.Text);
        }
        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}