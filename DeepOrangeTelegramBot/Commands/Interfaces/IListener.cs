using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Interfaces;

public interface IListener
{
    Task GetUpdate(Update update, TelegramBotClient client);
    public bool IsFinished { get; set; }
    public CommandExecutor Executor { get; }
}
