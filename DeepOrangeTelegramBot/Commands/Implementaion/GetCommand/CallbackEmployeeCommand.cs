using DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand.Models;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeepOrangeTelegramBot.Commands.Implementaion.GetCommand
{
    public class CallbackEmployeeCommand : ICommand
    {
        public string Name => "/callbackEmployee -u";

        private readonly OIdcService _oIdcService;

        public CallbackEmployeeCommand(OIdcService oIdcService)
        {
            _oIdcService = oIdcService;
        }

        public async Task Execute(Update update, TelegramBotClient client)
        {
            string pattern = @"-u(\S+)";

            if (update.CallbackQuery != null && update.CallbackQuery.Message != null && update.CallbackQuery.Data != null)
            {
                var employeeId = int.Parse(Regex.Match(update.CallbackQuery.Data, pattern).Groups[1].Value);
                var userId = update.CallbackQuery.From.Id;
                var chatId = update.CallbackQuery.Message.Chat.Id;
                var messageId = update.CallbackQuery.Message.MessageId;

                await GetDirectionInTgMessageAsync(chatId, userId, messageId, employeeId, client);
            }
        }

        private async Task GetDirectionInTgMessageAsync(long chatId, long userId, int messageId, int employeeId, TelegramBotClient client)
        {
            string requestUri = "https://localhost:7156/Employee/GetById?id=" + employeeId;

            var response = await _oIdcService.GetRequest(userId, requestUri);

            if (response is null)
            {
                await client.SendTextMessageAsync(chatId, "Ви не війшли у аккаунт!");
                return;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(jsonResponse);

            if (employee is null)
            {
                await client.SendTextMessageAsync(chatId, "Такої людини немає:/");
                return;
            }

            string msg = $"Ім'я: {employee.Name}\nПрізвище: {employee.LastName}\nСтек технологій: ";
            foreach(var technology in employee.Technologies) {
                msg += $"{technology.TechnologyName}, ";
            }

            await client.EditMessageTextAsync(chatId, messageId, msg);
        }
    }
}
