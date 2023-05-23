using API.Interfaces;
using Telegram.Bot.Types;

namespace API.Services
{
    public class UpdateDistributorService<T> where T: ITelegramUpdateListener, new()
    {
        private Dictionary<long, T> listeners = new Dictionary<long, T>();
        private readonly TelegramBot _bot;

        public UpdateDistributorService(TelegramBot bot)
        {
            _bot = bot;
        }

        public async Task GetUpdate(Update update)
        {
            long chatId = update.Message.Chat.Id;
            T? listener = listeners.GetValueOrDefault(chatId);
            if (listener is null)
            {
                listener = new T();
                listeners.Add(chatId, listener);
                await listener.GetUpdate(update);
                return;
            }
            await listener.GetUpdate(update);
        }
    }
}
