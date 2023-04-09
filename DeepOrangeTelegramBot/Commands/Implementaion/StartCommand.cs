using Telegram.Bot.Types;
using Telegram.Bot;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Bot.Interfaces;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class StartCommand : ICommand
{
    public string Name => "/start";

    private readonly TelegramBotClient _telegramBot;

    public StartCommand(ITelegramBot telegramBot)
    {
        _telegramBot = telegramBot.Client;
    }

    public async Task Execute(Update update)
    {
        if (update.Message is null)
            return;

        long chatId = update.Message.Chat.Id;
        await _telegramBot.SendTextMessageAsync(chatId, "Привет!");
    }
}