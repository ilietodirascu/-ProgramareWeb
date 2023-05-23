using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using NewsAPI.Constants;
using NewsAPI.Models;
using NewsAPI;
using Azure;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API
{
    public class TelegramBot
    {
        private readonly ILogger<TelegramBot> _logger;
        public TelegramBotClient Client { get; set; }
        private IDbContextFactory<DataContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        public TelegramBot(ILogger<TelegramBot> logger, IConfiguration configuration, IDbContextFactory<DataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _configuration = configuration;
            _ = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
               {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
               }
            };
            Client = new TelegramBotClient("6040930973:AAElR0DNudkORK26eRK9kXpIuB5LQOqnGns");

            using var cts = new CancellationTokenSource();

            _logger.LogInformation("Start listening for updates.");

        }
        public async Task HandleUpdate(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                // A message was received
                case UpdateType.Message:
                    await HandleMessage(update.Message!);
                    break;
            }
        }

        public async Task HandleError(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
        {
            await Console.Error.WriteLineAsync(exception.Message);
        }

        public async Task HandleMessage(Message msg)
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


        internal class Command
        {
            public string Instruction { get; set; }
            public string OptionalParam { get; set; }
        }
        private static Command GetCommand(string command)
        {
            var deconstructedCommand = command.Split(" ");
            var instruction = deconstructedCommand[0];
            var optionalParam = deconstructedCommand.Length > 1 ? string.Join("+", deconstructedCommand[1..]) : string.Empty;
            return new Command { Instruction = instruction, OptionalParam = optionalParam };
        }
        public async Task HandleCommand(long userId, string input)
        {
            var command = GetCommand(input);
            switch (command.Instruction)
            {
                case "/start":
                    await HandleStart(userId);
                    break;
                case "/latest_news":
                    await HandleLatestNews(userId, command.OptionalParam);
                    break;
                case "/save_news":
                    await HandleSaveCommand(userId,command.OptionalParam);
                    break;
                case "/saved_news":
                    await HandleSavedNews(userId);
                    break;
                default:
                    await Client.SendTextMessageAsync(userId, "No such command");
                    break;
            }
            await Task.CompletedTask;
        }
        public async Task HandleStart(long userId)
        {
            await Client.SendTextMessageAsync(userId, "Welcome!");
        }
        public async Task HandleLatestNews(long userId, string topic)
        {
            var newsApiClient = new NewsApiClient(_configuration["APIKEY"]);
            ArticlesResult? articles;
            articles = topic == string.Empty ? newsApiClient.GetTopHeadlines(new TopHeadlinesRequest
            {
                Language = Languages.EN,
                PageSize = 5,
            }) :
            newsApiClient.GetEverything(new EverythingRequest
            {
                Q = topic,
                PageSize = 5,
                SortBy = SortBys.Relevancy
            });
            var response = "Sure thing pal. Here are top 5 links:\n";
            articles.Articles.ForEach(a => response += $"\n{a.Url}");
            if (articles.TotalResults < 1) response += "\nIt seems there isn't a lot of info on this topic.";
            await Client.SendTextMessageAsync(userId, response);
        }
        public async Task HandleSaveCommand(long userId, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                await Client.SendTextMessageAsync(userId, "You must specify the url");
                return;
            }
            using var db = await _dbContextFactory.CreateDbContextAsync();
            try
            {
                db.UserLinks.Add(new UserLink { URL = url, UserId = userId });
                await db.SaveChangesAsync();
                await Client.SendTextMessageAsync(userId, "You got it, boss.");
            }
            catch (Exception)
            {
                await Client.SendTextMessageAsync(userId, "You can't save duplicate urls");
            }

        }
        public async Task HandleSavedNews(long userId)
        {
            using var db = await _dbContextFactory.CreateDbContextAsync();
            var urls = db.UserLinks.Where(u => u.UserId == userId).ToList();
            if (!urls.Any())
            {
                await Client.SendTextMessageAsync(userId, "Looks like you don't have anything saved up yet.");
                return;
            }
            var response = "";
            urls.ForEach(u => response += $"\n{u.URL}");
            await Client.SendTextMessageAsync(userId, response);
        }
    }
}
