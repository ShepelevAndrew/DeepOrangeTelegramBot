using DeepOrangeTelegramBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class SendAuthLinkCommand : ICommand
{
    public TelegramBotClient? Client { get; set; }

    public string Name => "/auth";

    public async Task Execute(Update update)
    {
        if (Client is null || update.Message is null)
            return;

        var authLink = "https://127.0.0.1:7004/connect/authorize?" +
                       "client_id=deep-orange-id&" +
                       "response_type=code&" +
                       "scope=openid%20profile" +
                       "%20DeepOrangeApi&" +
                       "redirect_uri=http://localhost:5000/auth&start=&state=some-state-data";

        await Client.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: $"Будь ласка авторизуйтесь: <a href=\"{authLink}\">увіти</a>",
            parseMode: ParseMode.Html
            );
    }
}
