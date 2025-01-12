namespace Music_Processor.CLI.Commands;

public class CommandFactory
{
    private readonly IReadOnlyDictionary<int, ICommand> _commands;

    public CommandFactory(IEnumerable<ICommand> commands)
    {
        _commands = commands.ToDictionary(c => c.MenuNumber);
    }

    public ICommand GetCommand(int menuChoice)
    {
        if (_commands.TryGetValue(menuChoice, out var command))
        {
            return command;
        }

        throw new ArgumentOutOfRangeException(nameof(menuChoice));
    }

    public IReadOnlyCollection<ICommand> GetAllCommands() => _commands.Values.ToList();
}