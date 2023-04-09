using Telegram.Bot.Types;
using Telegram.Bot;

namespace DeepOrangeTelegramBot.Commands.Interfaces;

public interface ICommand
{
    string Name { get; }

    Task Execute(Update update);
}
