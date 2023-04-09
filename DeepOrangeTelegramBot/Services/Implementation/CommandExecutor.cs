using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Implementaion;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Implementation;
public class CommandExecutor : ITelegramUpdateListener
{
    private readonly List<ICommand> commands;
    private IListener? listener = null;

    private readonly OIdcService _oIdcService;

    public CommandExecutor(ITelegramBot client, OIdcService oIdcService)
    {
        _oIdcService = oIdcService;
        
        commands = new List<ICommand>()
        {
            new StartCommand(client),
            new RegisterCommand(this),
            new CreateInviteLinkCommand(client),
            new SendAuthLinkCommand(client, _oIdcService),
            new GetEmployeeCommand(client, _oIdcService)
        };
    }

    public async Task GetUpdateAsync(Update update)
    {
        if (listener == null)
        {
            await ExecuteCommand(update);
        }
        else
        {
            await listener.GetUpdate(update);
        }
    }

    private async Task ExecuteCommand(Update update)
    {
        if (update.Message is not null)
        {
            Message msg = update.Message;
            foreach (var command in commands)
            {
                if (command.Name == msg.Text)
                {
                    await command.Execute(update);
                    break;
                }
            }
        }
    }

    public void StartListen(IListener newListener)
    {
        listener = newListener;
    }

    public void StopListen()
    {
        listener = null;
    }

    private List<ICommand> GetCommands()
    {
        var types = AppDomain
                  .CurrentDomain
                  .GetAssemblies()
                  .SelectMany(assembly => assembly.GetTypes())
                  .Where(type => typeof(ICommand).IsAssignableFrom(type))
                  .Where(type => type.IsClass);

        List<ICommand> commands = new List<ICommand>();
        foreach (var type in types)
        {
            ICommand? command;
            if (typeof(IListener).IsAssignableFrom(type))
            {
                command = Activator.CreateInstance(type, this) as ICommand;
            }
            else
            {
                command = Activator.CreateInstance(type) as ICommand;
            }

            if (command != null)
            {
                commands.Add(command);
            }
        }
        return commands;
    }
}