using CliFx.Infrastructure;
using Wolverine;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand : ICommand
{
    protected readonly IMessageBus _messageBus;

    protected BaseMenuCommand(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public abstract ValueTask ExecuteAsync(IConsole console);
}