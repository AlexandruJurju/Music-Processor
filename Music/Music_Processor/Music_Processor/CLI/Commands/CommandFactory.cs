namespace Music_Processor.CLI.Commands;

public class CommandFactory
{
    private readonly IReadOnlyDictionary<int, IMenuCommand> _commands;

    public CommandFactory(IEnumerable<IMenuCommand> commands)
    {
        _commands = commands.ToDictionary(c => c.MenuNumber);
    }

    public IMenuCommand GetCommand(int menuChoice)
    {
        if (_commands.TryGetValue(menuChoice, out var command))
        {
            return command;
        }

        throw new ArgumentOutOfRangeException(nameof(menuChoice));
    }

    public IReadOnlyCollection<IMenuCommand> GetAllCommands() => _commands.Values.ToList();
}