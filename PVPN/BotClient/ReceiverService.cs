using PVPN.BotClient.Abstract;
using Telegram.Bot;

namespace PVPN.BotClient;

// Compose Receiver and UpdateHandler implementation
public class ReceiverService : ReceiverServiceBase<UpdateHandler>
{
    public ReceiverService(
        ITelegramBotClient botClient,
        UpdateHandler updateHandler)
        : base(botClient, updateHandler)
    {
    }
}
