using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Interfaces;

public interface IListener
{
    Task GetUpdate(Update update);
    public bool IsFinished { get; set; }
    public CommandExecutor Executor { get; }
}
