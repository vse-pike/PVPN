using Telegram.Bot.Types;

namespace PVPN.BotClient.Commands;

public class UnknownCommand: ICommand
{
    
    //TODO: Implement later
    public Task Execute(Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}