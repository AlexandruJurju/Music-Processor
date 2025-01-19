namespace MusicProcessor.CLI.MenuOptions.Abstractions;

/// <summary>
/// Builds the available menu options
/// </summary>
public class MenuCommandFactory
{
    private readonly Dictionary<int, IMenuOption> _commands;

    public MenuCommandFactory(IEnumerable<IMenuOption> commands)
    {
        _commands = commands.Select((cmd, index) => new { cmd, index = index + 1 })
            .ToDictionary(x => x.index, x => x.cmd);
    }

    public IMenuOption GetCommand(int menuChoice)
    {
        if (_commands.TryGetValue(menuChoice, out var command))
        {
            return command;
        }

        throw new ArgumentOutOfRangeException(nameof(menuChoice));
    }

    public IReadOnlyCollection<KeyValuePair<int, IMenuOption>> GetAllCommands()
    {
        return _commands.ToList();
    }
}