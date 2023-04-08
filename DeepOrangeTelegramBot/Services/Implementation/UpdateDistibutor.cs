using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Implementation;

public class UpdateDistributor<T> where T : ITelegramUpdateListener, new()
{
    private readonly Dictionary<long, T> listeners;
    private readonly TelegramBotClient telegramClient;

    public UpdateDistributor(ITelegramBot telegramBot)
    {
        listeners = new Dictionary<long, T>();
        telegramClient = telegramBot.Client;
    }

    public async Task GetUpdateAsync(Update update)
    {
        if (update.Message is not null)
        {
            long chatId = update.Message.Chat.Id;
            T? listener = listeners.GetValueOrDefault(chatId);
            if (listener is null)
            {
                listener = new T
                {
                    Client = telegramClient
                };
                listeners.Add(chatId, listener);
                await listener.GetUpdateAsync(update);
                return;
            }
            await listener.GetUpdateAsync(update);
        }
    }
}