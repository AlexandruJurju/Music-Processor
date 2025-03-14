using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.CommitChanges;
using MusicProcessor.Infrastructure.FileService;

namespace MusicProcessor.CLI.MenuCommands;

[Command("commit", Description = "Commit changes to the library")]
public class CommitChanges : BaseMenuCommand
{
    public CommitChanges(
        IFileService fileService,
        IMediator mediator
    ) : base(fileService, mediator)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _mediator.Send(new CommitChangesCommand());
    }
}