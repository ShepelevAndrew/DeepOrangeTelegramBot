using DeepOrangeTelegramBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class CreateInviteLinkCommand : ICommand
{
    public TelegramBotClient? Client { get; set; }

    public string Name => "/invite";

    private const long deepOrangeChateId = -1001904708878;

    public async Task Execute(Update update)
    {
        if (Client is null || update.Message is null)
            return;

        var chatId = update.Message.Chat.Id;

        var inviteLink = await Client.CreateChatInviteLinkAsync(
                                            chatId: deepOrangeChateId,
                                            name: "privateChatLink",
                                            expireDate: DateTime.Now.AddDays(1),
                                            memberLimit: 1,
                                            createsJoinRequest: false
                                            );

        await Client.SendTextMessageAsync(
                chatId: chatId,
                text: inviteLink.InviteLink
                );
    }
}