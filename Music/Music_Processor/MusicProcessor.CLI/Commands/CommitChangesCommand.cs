using System.CommandLine;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.CommitChangesToLibrary;

namespace MusicProcessor.CLI.Commands;

public class CommitChangesCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override Command CreateSubCommand()
    {
        var command = new Command("commit", "Commit changes to the library");

        command.SetHandler(async () => { await Mediator.Send(new CommitChangesToLibraryCommand()); });

        return command;
    }
}