using PVPN.BotClient.Commands;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PVPN.BotClient;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly CommandContainer _commandContainer;
    private const string CommandPrefix = "/";
    
    public UpdateHandler(ITelegramBotClient botClient, CommandContainer container)
    {
        _botClient = botClient;
        _commandContainer = container;
    }

  
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:

                var message = update.Message?.Text;
                if (message == null)
                {
                    break;
                }

                if (message.StartsWith(CommandPrefix))
                {
                    var commandIdentifier = message.Substring(1);

                    await _commandContainer.RetrieveCommand(commandIdentifier).Execute(update, cancellationToken);
                }

                break;
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}