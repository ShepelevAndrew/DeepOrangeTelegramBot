using Telegram.Bot;

namespace DeepOrangeTelegramBot.Bot.Interfaces;

public interface ITelegramBot
{
    public TelegramBotClient Client { get; set; }
}
