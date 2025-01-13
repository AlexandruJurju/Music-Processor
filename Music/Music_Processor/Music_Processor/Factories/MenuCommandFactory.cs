using Music_Processor.CLI.Commands;

namespace Music_Processor.Factories;

public class MenuCommandFactory
{
    private readonly Dictionary<int, IMenuCommand> _commands;

    public MenuCommandFactory(IEnumerable<IMenuCommand> commands)
    {
        _commands = commands.Select((cmd, index) => new { cmd, index = index + 1 })
            .ToDictionary(x => x.index, x => x.cmd);
    }

    public IMenuCommand GetCommand(int menuChoice)
    {
        if (_commands.TryGetValue(menuChoice, out var command))
        {
            return command;
        }

        throw new ArgumentOutOfRangeException(nameof(menuChoice));
    }

    public IReadOnlyCollection<KeyValuePair<int, IMenuCommand>> GetAllCommands()
    {
        return _commands.ToList();
    }
}