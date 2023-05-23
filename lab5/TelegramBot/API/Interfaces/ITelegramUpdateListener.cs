using Telegram.Bot.Types;

namespace API.Interfaces
{
    public interface ITelegramUpdateListener
    {
        Task GetUpdate(Update update);
    }
}
