using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class SendAuthLinkCommand : ICommand
{
    public string Name => "/auth";

    private readonly TelegramBotClient _telegramBot;
    private readonly OIdcService _oIdcService;

    public SendAuthLinkCommand(ITelegramBot telegramBot, OIdcService oIdcService)
    {
        _telegramBot = telegramBot.Client;
        _oIdcService = oIdcService;
    }

    public async Task Execute(Update update)
    {
        if (update.Message is null || update.Message.From is null)
            return;

        var userId = update.Message.From.Id;
        var chatId = update.Message.Chat.Id;

        var userInfo = await _oIdcService.FindUserInfoAsync(userId);

        if (userInfo is not null)
            await GreetAsync(userInfo, chatId);
        else
            await AskForLoginAsync(userId, chatId);
    }

    private async Task GreetAsync(UserInfo userInfo, long chatId)
    {
        var username = userInfo.PreferredUsername;
        var message = $"Hello, <b>{username}</b>!\nYou are the best! Have a nice day!";

        await _telegramBot.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            parseMode: ParseMode.Html
        );
    }

    private async Task AskForLoginAsync(long userId, long chatId)
    {
        var url = _oIdcService.GetAuthUrl(userId);
        var message = $"Будь ласка авторизуйтесь: <a href=\"{url}\">увіти</a>";

        await _telegramBot.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            parseMode: ParseMode.Html
        );
    }
}
