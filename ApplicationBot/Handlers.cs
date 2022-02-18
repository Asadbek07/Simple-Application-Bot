using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ApplicationBot
{
    public class Handlers
    {
        public static List<UserData> users = new();
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch

            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                 _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);

            }
        }
        public static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start")
            {
                var user = new UserData(message.From.Id, message.From.Username);
                users.Add(user);
                user.State = UserRegisterState.SendingName;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter your name :");
            }
            else
            {
                var user = users.FirstOrDefault(user => user.ChatId == message.From.Id);
                
                if (user.State == UserRegisterState.None)
                {
                    user.State = UserRegisterState.SendingName;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "You are about to fill this application one more time!\nPlease, enter your name: ");
                }
                else if (user.State == UserRegisterState.SendingName)
                {
                    user.Name = message.Text;
                    user.State = UserRegisterState.SendingSurname;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter your surname: ");
                }
                else if (user.State == UserRegisterState.SendingSurname)
                {
                    user.Surname = message.Text;
                    user.State = UserRegisterState.SendingPhoneNumber;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter your phone number: ");
                }
                else if (user.State == UserRegisterState.SendingPhoneNumber)
                {
                    user.PhoneNumber = message.Text;
                    user.State = UserRegisterState.SendingPassword;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter new password: ");
                }
                else if (user.State == UserRegisterState.SendingPassword)
                {
                    user.Password = message.Text;
                    user.State = UserRegisterState.None;
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"<b>Your data</b>: \nName: {user.Name}\nSurname: {user.Surname}\nPhone Number: {user.PhoneNumber}\nPassword: {user.Password}", ParseMode.Html);
                }

            }
        }
            
        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

    }
}
