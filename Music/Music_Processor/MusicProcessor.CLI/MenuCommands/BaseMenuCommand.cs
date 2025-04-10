using CliFx.Infrastructure;
using MediatR;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand(ISender sender) : ICommand
{
    protected readonly ISender _sender = sender;

    public abstract ValueTask ExecuteAsync(IConsole console);
}
