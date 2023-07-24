using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class CreateInviteLinkCommand : ICommand
{
    public string Name => "/invite";

    private const long deepOrangeChateId = -1001904708878;

    public async Task Execute(Update update, TelegramBotClient client)
    {
        if (update.Message is null)
            return;

        var chatId = update.Message.Chat.Id;

        var inviteLink = await client.CreateChatInviteLinkAsync(
                                            chatId: deepOrangeChateId,
                                            name: "privateChatLink",
                                            expireDate: DateTime.Now.AddDays(1),
                                            memberLimit: 1,
                                            createsJoinRequest: false
                                            );

        await client.SendTextMessageAsync(
                chatId: chatId,
                text: inviteLink.InviteLink
                );
    }
}