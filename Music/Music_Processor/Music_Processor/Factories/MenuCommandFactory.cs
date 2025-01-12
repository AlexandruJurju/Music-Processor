using Music_Processor.CLI.Commands;

namespace Music_Processor.Factories;

public class MenuCommandFactory
{
    private readonly IReadOnlyDictionary<int, IMenuCommand> _commands;

    public MenuCommandFactory(IEnumerable<IMenuCommand> commands)
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