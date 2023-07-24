using DeepOrangeTelegramBot.Bot.Interfaces;
using DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand.Models;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand;

public class GetCommand : ICommand
{
    public string Name => "/get /get@DeepOrange_bot -сget";

    private readonly OIdcService _oIdcService;

    public GetCommand(OIdcService oIdcService)
    {
        _oIdcService = oIdcService;
    }

    public async Task Execute(Update update, TelegramBotClient client)
    {
        if(update.Message != null && update.Message.From != null)
        {
            var userId = update.Message.From.Id;
            var chatId = update.Message.Chat.Id;

            await GetEmployeesInTgMessageAsync(chatId, userId, client);
        }
    }

    private async Task GetEmployeesInTgMessageAsync(long chatId, long userId, TelegramBotClient client)
    {
        string requestUri = "https://localhost:7156/Employee/Get";

        var response = await _oIdcService.GetRequest(userId, requestUri);

        if (response is null)
        {
            await client.SendTextMessageAsync(chatId, "Ви не війшли у аккаунт!");
            return;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var employees = JsonConvert.DeserializeObject<List<Employee>>(jsonResponse);

        if(employees is null)
        {
            await client.SendTextMessageAsync(chatId, "Напряму немає:/");
            return;
        }

        var buttons = new List<InlineKeyboardButton>();
        foreach (var employee in employees)
        {
            buttons.Add(InlineKeyboardButton.WithCallbackData(employee.Name, $"/callbackEmployee -u{employee.EmployeeId}"));
        }

        var inlineKeyboard = new InlineKeyboardMarkup(buttons);

        await client.SendTextMessageAsync(chatId, "Обери ім'я:", replyMarkup: inlineKeyboard);
    }
}
