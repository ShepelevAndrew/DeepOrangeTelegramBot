using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class CreateInviteLinkCommand : ICommand
{
    public string Name => "/invite";

    private const long deepOrangeChateId = -1001904708878;

    private readonly TelegramBotClient _telegramBot;

    public CreateInviteLinkCommand(ITelegramBot telegramBot)
    {
        _telegramBot = telegramBot.Client;
    }

    public async Task Execute(Update update)
    {
        if (update.Message is null)
            return;

        var chatId = update.Message.Chat.Id;

        var inviteLink = await _telegramBot.CreateChatInviteLinkAsync(
                                            chatId: deepOrangeChateId,
                                            name: "privateChatLink",
                                            expireDate: DateTime.Now.AddDays(1),
                                            memberLimit: 1,
                                            createsJoinRequest: false
                                            );

        await _telegramBot.SendTextMessageAsync(
                chatId: chatId,
                text: inviteLink.InviteLink
                );
    }
}