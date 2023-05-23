using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace API.BotController
{
    [ApiController] 
    [Route("/update")]
    public class HandleUpdateEndpoint : ControllerBase
    {
        private readonly TelegramBot _bot;

        public HandleUpdateEndpoint(TelegramBot bot)
        {
            _bot = bot;
        }

        [HttpPost]
        public async Task Post(Update update, CancellationToken ct)
        {
            await _bot.HandleUpdate(_bot.Client, update, ct);
        }
    }
}
