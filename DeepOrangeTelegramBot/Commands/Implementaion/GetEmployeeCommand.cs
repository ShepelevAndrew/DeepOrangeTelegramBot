using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DeepOrangeTelegramBot.Commands.Implementaion;

public class GetEmployeeCommand : ICommand
{
    public string Name => "/get -qEmloyees -qDirections -qTechnologies -n/get@DeepOrangeBot -nget";

    private readonly TelegramBotClient _telegramBot;
    private readonly OIdcService _oIdcService;

    public GetEmployeeCommand(ITelegramBot telegramBot, OIdcService oIdcService)
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

        var requestUri = "https://localhost:7156/Employee/Get";

        var response = await _oIdcService.GetRequest(userId, requestUri);

        if(response is null) { 
            await _telegramBot.SendTextMessageAsync(chatId, "Ви не війшли у аккаунт!");
            return;
        }

        var employee = await response.Content.ReadAsStringAsync();

        await _telegramBot.SendTextMessageAsync(chatId, employee);
    }
}
