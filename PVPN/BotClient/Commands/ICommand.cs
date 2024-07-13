using Telegram.Bot.Types;

namespace PVPN.BotClient.Commands;

public interface ICommand
{
    public Task Execute(Update update, CancellationToken cancellationToken);
}