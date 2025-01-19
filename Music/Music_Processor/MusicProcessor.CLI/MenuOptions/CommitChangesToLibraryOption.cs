using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.CommitChangesToLibrary;
using MusicProcessor.CLI.MenuOptions.Abstractions;

namespace MusicProcessor.CLI.MenuOptions;

public class CommitChangesToLibraryOption(IFileService fileService, IMediator mediator) : BaseMenuOption(fileService, mediator)
{
    public override string Name => "CommitChanges";

    public override async Task ExecuteAsync()
    {
        await _mediator.Send(new CommitChangesToLibraryCommand());
    }
}