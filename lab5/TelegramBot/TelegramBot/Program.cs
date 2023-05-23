using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using static System.Net.Mime.MediaTypeNames;


var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = new UpdateType[]
               {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
               }
};
var bot = new TelegramBotClient("6040930973:AAElR0DNudkORK26eRK9kXpIuB5LQOqnGns");

using var cts = new CancellationTokenSource();

bot.StartReceiving(HandleUpdate,HandleError,receiverOptions);

Console.WriteLine("Start listening for updates. Press enter to stop");
Console.ReadLine();

cts.Cancel();

// Each time a user interacts with the bot, this method is called
async Task HandleUpdate(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
{
    switch (update.Type)
    {
        // A message was received
        case UpdateType.Message:
            await HandleMessage(update.Message!);
            break;
    }
}

async Task HandleError(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
{
    await Console.Error.WriteLineAsync(exception.Message);
}

async Task HandleMessage(Message msg)
{
    var user = msg.From;
    var text = msg.Text ?? string.Empty;

    if (user is null)
        return;

    // Print to console
    Console.WriteLine($"{user.FirstName} wrote {text}");

    // When we get a command, we react accordingly
    if (text.StartsWith("/"))
    {
        await HandleCommand(user.Id, text);
    }
}


async Task HandleCommand(long userId, string command)
{
    switch (command)
    {
        case "/start":
            await HandleStart(userId);
            break;

        case "/latest_news":
            break;
        case "/save_news":
            break;
        case "/saved_news":
            break;
        default:
            await bot.SendTextMessageAsync(userId, "No such command");
            break;
    }
    await Task.CompletedTask;
}
async Task HandleStart(long userId)
{
    await bot.SendTextMessageAsync(userId, "Welcome!");
}

