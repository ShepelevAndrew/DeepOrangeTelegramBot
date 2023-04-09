using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Interfaces;

public interface ITelegramUpdateListener
{
    Task GetUpdateAsync(Update update);
}