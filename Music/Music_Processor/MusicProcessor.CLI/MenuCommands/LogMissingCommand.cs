using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.LogMissing;

namespace MusicProcessor.CLI.MenuCommands;

[Command("log-missing", Description = "Log missing songs that don't have metadata")]
public class LogMissingCommand : BaseMenuCommand
{
    public LogMissingCommand(IFileService fileService, IMediator mediator) : base(fileService, mediator)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _mediator.Send(new LogMissingQuery());
    }
}