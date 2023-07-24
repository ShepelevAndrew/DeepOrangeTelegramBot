using DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand.Models;
using DeepOrangeTelegramBot.Commands.Interfaces;
using DeepOrangeTelegramBot.Services.Implementation;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand
{
    public class CreateCommand : ICommand, IListener
    {
        public string Name => "/create /create@DeepOrange_bot";

        public bool IsFinished { get; set; } = false;

        public CommandExecutor Executor { get; }

        private readonly OIdcService _oIdcService;

        public CreateCommand(OIdcService oIdcService, CommandExecutor executor)
        {
            _oIdcService = oIdcService;
            Executor = executor;
        }

        private string? employeeName;
        private string? employeeLastName;
        private int? directionId;
        private int? technologyId;


        public async Task Execute(Update update, TelegramBotClient client)
        {
            long chatId;
            if (update.Message is not null)
                chatId = update.Message.Chat.Id;
            else
                chatId = update.CallbackQuery.Message.Chat.Id;

            if (IsFinished)
            {
                await client.SendTextMessageAsync(chatId, "Аккаунт вже добавлено!");
                return;
            }

            Executor.StartListen(this);
            await client.SendTextMessageAsync(chatId, "Напишіть своє ім'я!");
        }

        public async Task GetUpdate(Update update, TelegramBotClient client)
        {
            long chatId, userId;
            if (update.Message is not null) { 
                chatId = update.Message.Chat.Id;
                userId = update.Message.From.Id;
            }
            else { 
                chatId = update.CallbackQuery.Message.Chat.Id;
                userId = update.CallbackQuery.From.Id;
            }

            if (employeeName == null)
            {
                employeeName = update.Message.Text;
                await client.SendTextMessageAsync(chatId, "Напишіть прізвище!");
            }
            else if (employeeLastName == null)
            {
                employeeLastName = update.Message.Text;
                string requestUri = "https://localhost:7156/Direction/Get";

                var response = await _oIdcService.GetRequest(userId, requestUri);

                if (response is null)
                {
                    await client.SendTextMessageAsync(chatId, "Ви не війшли у аккаунт!");
                    return;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var directions = JsonConvert.DeserializeObject<List<Direction>>(jsonResponse);

                if (directions is null)
                {
                    await client.SendTextMessageAsync(chatId, "Напряму немає:/");
                    return;
                }

                var buttons = new List<InlineKeyboardButton>();
                foreach (var direction in directions)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(direction.DirectionName, $"{direction.DirectionId}"));
                }

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    buttons
                });

                await client.SendTextMessageAsync(chatId, "Обери напрям", replyMarkup: inlineKeyboard);
            }
            else if(directionId == null)
            {
                directionId = int.Parse(update.CallbackQuery.Data);

                string requestUri = "https://localhost:7156/Technology/GetByDirectionId?id=" + directionId;

                var response = await _oIdcService.GetRequest(userId, requestUri);

                if (response is null)
                {
                    await client.SendTextMessageAsync(chatId, "Ви не війшли у аккаунт!");
                    return;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var technologies = JsonConvert.DeserializeObject<List<Technology>>(jsonResponse);

                if (technologies is null)
                {
                    await client.SendTextMessageAsync(chatId, "Такої технології немає:/");
                    return;
                }

                var buttons = new List<InlineKeyboardButton>();
                foreach (var technology in technologies)
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData(technology.TechnologyName, $"{technology.TechnologyId}"));
                }

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    buttons
                });

                var msgId = update.CallbackQuery.Message.MessageId;
                await client.EditMessageTextAsync(chatId, msgId, "Обери технологію", replyMarkup: inlineKeyboard);
            }
            else if(technologyId == null) {

                technologyId = int.Parse(update.CallbackQuery.Data);

                string requestUri = "https://localhost:7156/Employee/Create";

                var employee = new Employee
                {
                    EmployeeId = 16,
                    Name = employeeName,
                    LastName = employeeLastName,
                    Technologies = new List<Technology>()
                    {
                        new Technology
                        {
                            TechnologyId = (int)technologyId
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(employee);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _oIdcService.PostRequestAsync(userId, requestUri, content);
                
                if(response.IsSuccessStatusCode) {
                    await client.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, "Вітаю ви добавили аккаунт!");

                    IsFinished = true;
                    Executor.StopListen();
                }
                else
                {
                    await client.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, $"Похибка! {response.StatusCode} - {response.ReasonPhrase}");

                    IsFinished = false;
                    Executor.StopListen();
                }
            }
        }
    }
}