using Telegram.Bot.Types;
using Telegram.Bot;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Bot.Interfaces;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class StartCommand : ICommand
{
    public string Name => "/start /start@DeepOrange_bot";

    public async Task Execute(Update update, TelegramBotClient telegramBot)
    {
        if (update.Message is null)
            return;

        long chatId = update.Message.Chat.Id;
        await telegramBot.SendTextMessageAsync(chatId, "Привет!");
    }
}