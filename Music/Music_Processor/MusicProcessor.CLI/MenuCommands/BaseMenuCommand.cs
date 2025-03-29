using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand : ICommand
{
    protected readonly IMediator _mediator;

    protected BaseMenuCommand(
        IMediator mediator
    )
    {
        _mediator = mediator;
    }

    public abstract ValueTask ExecuteAsync(IConsole console);
}