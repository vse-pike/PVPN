using PVPN.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PVPN.BotClient.Commands;

public class AddAccessCommand: ICommand
{
    
    private readonly ITelegramBotClient _botClient;
    private readonly IHandler _handler;
    private static readonly Random Random = new();

    public AddAccessCommand(ITelegramBotClient botClient, IHandler handler)
    {
        _botClient = botClient;
        _handler = handler;
    }
    
    public async Task Execute(Update update, CancellationToken cancellationToken)
    {
        var telegramId = update.Message?.From?.Id;

        if (telegramId != null)
        {
            try
            {
                var exist = await _handler.CheckUserShouldExist(telegramId.Value);
                if (exist)
                {
                    //TODO: Добавить проверку существования порта
                    var port = Random.Next(1000, 10000);
                    var password = GenerateRandomPassword(8);
                    
                    await _handler.AddVpnAccess(telegramId.Value, port, password);
                }
                else
                {
                    throw new Exception("User is not exist");
                }
            }
            catch (Exception e)
            {
                await _botClient.SendTextMessageAsync(telegramId, e.Message, cancellationToken: cancellationToken);
            }
        }
    }
    
    private static string GenerateRandomPassword(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        char[] password = new char[length];
        for (var i = 0; i < length; i++)
        {
            password[i] = chars[Random.Next(chars.Length)];
        }

        return new string(password);
    }
}