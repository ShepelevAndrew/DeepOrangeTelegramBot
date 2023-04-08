using Telegram.Bot.Types;
using Telegram.Bot;
using DeepOrangeTelegramBot.Bot;
using DeepOrangeTelegramBot.Commands.Interfaces;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class StartCommand : ICommand
{
    public TelegramBotClient? Client { get; set; }

    public string Name => "/start";

    public async Task Execute(Update update)
    {
        if (Client is null || update.Message is null)
            return;

        long chatId = update.Message.Chat.Id;
        await Client.SendTextMessageAsync(chatId, "Привет!");
    }
}