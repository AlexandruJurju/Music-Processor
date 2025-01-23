using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.CommitChangesToLibrary;

namespace MusicProcessor.CLI.MenuCommands;

[Command("commit", Description = "Commit changes to the library")]
public class CommitChangesCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await Mediator.Send(new CommitChangesToLibraryCommand());
    }
}