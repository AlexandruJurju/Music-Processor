using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.CLI.MenuOptions.Abstractions;

namespace MusicProcessor.CLI.MenuOptions;

public class TestWriteToDBWithSpotdlMeta(IFileService fileService, IMediator mediator) : BaseMenuOption(fileService, mediator)
{
    public override string Name => "TestWriteToDBWithSpotdlMeta";

    public override async Task ExecuteAsync()
    {
        var playlistName = GetValidatedPlaylistName();
        if (playlistName == null) return;

        var playlistPath = GetPlaylistPath(playlistName);
        await _mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}