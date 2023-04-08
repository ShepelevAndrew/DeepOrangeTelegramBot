using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Interfaces;

public interface ITelegramUpdateListener
{
    TelegramBotClient Client { get; init; }
    Task GetUpdateAsync(Update update);
}