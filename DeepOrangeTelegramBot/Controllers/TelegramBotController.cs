using DeepOrangeTelegramBot.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Controllers
{
    [ApiController]
    [Route("/")]
    public class TelegramBotController : ControllerBase
    {
        private readonly UpdateDistributor<CommandExecutor> _updateDistributor;

        public TelegramBotController(UpdateDistributor<CommandExecutor> updateDistributor)
        {
            _updateDistributor = updateDistributor;
        }

        [HttpPost]
        public async void Post(Update update)
        {
            if (update.Message == null)
                return;

            await _updateDistributor.GetUpdateAsync(update);
        }
        [HttpGet]
        public string Get()
        {
            return "Telegram bot was started";
        }
    }
}