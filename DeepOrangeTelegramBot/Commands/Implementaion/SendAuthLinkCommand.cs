using DeepOrangeTelegramBot.Bot;
using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class SendAuthLinkCommand : ICommand
{
    public string Name => "/auth /auth@DeepOrange_bot /start -pauth";

    private readonly OIdcService _oIdcService;

    public SendAuthLinkCommand(OIdcService oIdcService)
    {
        _oIdcService = oIdcService;
    }

    public async Task Execute(Update update, TelegramBotClient client)
    {
        if (update.Message is null || update.Message.From is null)
            return;

        var userId = update.Message.From.Id;
        var chatId = update.Message.Chat.Id;

        var userInfo = await _oIdcService.FindUserInfoAsync(userId);

        if (userInfo is not null)
            await GreetAsync(userInfo, chatId, client);
        else
            await AskForLoginAsync(userId, chatId, client);
    }

    private async Task GreetAsync(UserInfo userInfo, long chatId, TelegramBotClient client)
    {
        var username = userInfo.PreferredUsername;
        var message = $"Hello, <b>{username}</b>!\nYou are the best! Have a nice day!";

        await client.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            parseMode: ParseMode.Html
        );
    }

    private async Task AskForLoginAsync(long userId, long chatId, TelegramBotClient client)
    {
        var url = _oIdcService.GetAuthUrl(userId);
        var message = $"Будь ласка авторизуйтесь: <a href=\"{url}\">увійти</a>";

        await client.SendTextMessageAsync(
            chatId: chatId,
            text: message,
            parseMode: ParseMode.Html
        );
    }
}
