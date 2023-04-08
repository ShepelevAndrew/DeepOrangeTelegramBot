using Telegram.Bot.Types;
using Telegram.Bot;

namespace DeepOrangeTelegramBot.Commands.Interfaces;

public interface ICommand
{
    TelegramBotClient Client { get; set; }

    string Name { get; }

    Task Execute(Update update);
}
