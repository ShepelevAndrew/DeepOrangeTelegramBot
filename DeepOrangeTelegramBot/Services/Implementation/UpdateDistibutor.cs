using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class UpdateDistributor
{
    private readonly Dictionary<long, CommandExecutor> listeners;

    private readonly ITelegramBot _telegramBot;
    private readonly OIdcService _oIdcService;

    public UpdateDistributor(ITelegramBot telegramBot, OIdcService oIdcService)
    {
        listeners = new Dictionary<long, CommandExecutor>();
        _telegramBot = telegramBot;
        _oIdcService = oIdcService;
    }

    public async Task GetUpdateAsync(Update update)
    {
        if (update.Message is not null)
        {
            long chatId = update.Message.Chat.Id;
            var listener = listeners.GetValueOrDefault(chatId);
            if (listener is null)
            {
                listener = new CommandExecutor(_telegramBot, _oIdcService);
                listeners.Add(chatId, listener);
                await listener.GetUpdateAsync(update);
                return;
            }
            await listener.GetUpdateAsync(update);
        }
    }
}