namespace PVPN.BotClient.Commands;

public class CommandContainer
{
    private readonly Dictionary<string, ICommand> _commandMap;
    private readonly ICommand _unknownCommand;
    
    public CommandContainer(
        StartCommand startCommand,
        PayCommand payCommand,
        AddAccessCommand accessCommand
    ) {

        _commandMap = new Dictionary<string, ICommand>
        {
            { "start", startCommand },
            { "pay", payCommand },
            { "access", accessCommand},
        };

        _unknownCommand = new UnknownCommand();
    }
    
    public ICommand RetrieveCommand(string commandIdentifier)
    {
        if (_commandMap.TryGetValue(commandIdentifier, out ICommand? command))
        {
            return command;
        }

        return _unknownCommand;
    }
}