using Telegram.Bot.Types;
using Telegram.Bot;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class RegisterCommand : ICommand, IListener
{
    public string Name => "/register";
    public bool IsFinished { get; set; } = false;

    public CommandExecutor Executor { get; }

    public RegisterCommand(CommandExecutor executor)
    {
        Executor = executor;
    }

    private string? phone = null;
    private string? name = null;

    public async Task Execute(Update update, TelegramBotClient client)
    {
        if (update.Message is null)
            return;

        long chatId = update.Message.Chat.Id;

        if(IsFinished)
        {
            await client.SendTextMessageAsync(chatId, "Ви зареєстровані!");
            return;
        }

        Executor.StartListen(this);
        await client.SendTextMessageAsync(chatId, "Напишіть свій номер!");

    }

    public async Task GetUpdate(Update update, TelegramBotClient client)
    {
        if(update.Message is null) 
            return;

        long chatId = update.Message.Chat.Id;

        if (phone == null) {
            phone = update.Message.Text;
            await client.SendTextMessageAsync(chatId, "Напишіть своє ім'я!");
        } 
        else {
            name = update.Message.Text;
            await client.SendTextMessageAsync(chatId, "Вітаю ви зареєструвались!");

            IsFinished = true;
            Executor.StopListen();
        }
    }
}