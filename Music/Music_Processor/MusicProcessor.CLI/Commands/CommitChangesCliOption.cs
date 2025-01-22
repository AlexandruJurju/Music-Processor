using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.CommitChangesToLibrary;

namespace MusicProcessor.CLI.Commands;

[Command("commit", Description = "Commit changes to the library")]
public class CommitChangesCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await mediator.Send(new CommitChangesToLibraryCommand());
    }
}