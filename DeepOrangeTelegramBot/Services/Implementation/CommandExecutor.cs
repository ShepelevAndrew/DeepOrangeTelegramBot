using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Implementaion;
using DeepOrangeTelegramBot.Commands.Implementaion.GetCommand;
using DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Interfaces;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Services.Implementation;
public class CommandExecutor : ITelegramUpdateListener
{
    private IListener? listener = null;
    private readonly List<ICommand> commands;
    private readonly TelegramBotClient _client;

    public CommandExecutor(ITelegramBot telegramBot, OIdcService oIdcService)
    {
        _client = telegramBot.Client;
        
        commands = new List<ICommand>()
        {
            new StartCommand(),
            new RegisterCommand(this),
            new CreateInviteLinkCommand(),
            new SendAuthLinkCommand(oIdcService),
            new GetCommand(oIdcService),
            new CreateCommand(oIdcService, this),
            new CallbackEmployeeCommand(oIdcService)
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
            await listener.GetUpdate(update, _client);
        }
    }

    private async Task ExecuteCommand(Update update)
    {
        string commandName = "";
        if (update.Message is not null && update.Message.Text is not null)
        {
            commandName = update.Message.Text;
        }
        else if(update.CallbackQuery is not null && update.CallbackQuery.Data is not null)
        {
            commandName = update.CallbackQuery.Data;
        }

        string pattern = @"-u\S*";
        commandName = Regex.Replace(commandName, pattern, "-u");

        foreach (var command in commands)
        {
            var queries = GetQueriesFromCommandName(command.Name);
            foreach (var query in queries) {
                if (query == commandName)
                {
                    await command.Execute(update, _client);
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

    private static List<string> GetQueriesFromCommandName(string commandName)
    {
        string[] words = commandName.Split(' ');

        string query = "";
        var queries = new List<string>();

        string commandSymb = "-с", parametrSymb = "-p", unknownParametrSymb = "-u";

        foreach (string word in words)
        {
            if (word.StartsWith(commandSymb) || word.StartsWith('/'))
            {
                var command = word.Replace(commandSymb, "");

                query = "";
                query += command;

                queries.Add(command);
            }
            else if (word.StartsWith(parametrSymb))
            {
                string command = query;
                queries.Remove(command);

                var parametr = word.Replace(parametrSymb, "");

                query += " " + parametr;
                queries.Add(query);

                query = command;
            }
            else if (word.StartsWith(unknownParametrSymb))
            {
                string command = query;
                queries.Remove(command);

                query += " " + unknownParametrSymb;
                queries.Add(query);

                query = command;
            }
        }

        return queries;
    }
}