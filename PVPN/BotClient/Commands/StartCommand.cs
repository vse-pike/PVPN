using PVPN.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = PVPN.Data.Model.User;

namespace PVPN.BotClient.Commands;

public class StartCommand: ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IHandler _handler;
    
    public StartCommand(ITelegramBotClient botClient, IHandler handler)
    {
        _botClient = botClient;
        _handler = handler;
    }
    
    public async Task Execute(Update update, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = update.Message?.From?.Username ?? "Unknown",
            TelegramId = update.Message!.From!.Id,
            CreatedDate = DateTime.Now
        };
                    
        try
        {
            await _handler.CreateUser(user);
        }
        catch (Exception e)
        {
            await _botClient.SendTextMessageAsync(user.TelegramId, e.Message, cancellationToken: cancellationToken);
        }
    }
}