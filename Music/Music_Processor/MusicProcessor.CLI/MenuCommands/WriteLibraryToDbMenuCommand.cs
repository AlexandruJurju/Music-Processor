using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

namespace MusicProcessor.CLI.MenuCommands;

[Command("write-db", Description = "Write a library metadata to the database")]
public class WriteLibraryToDbMenuCommand : BaseMenuCommand
{
    public WriteLibraryToDbMenuCommand(IFileService fileService, IMediator mediator) : base(fileService, mediator)
    {
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public required string PlaylistName { get; init; }

    [CommandOption("use-spotdl", 's', IsRequired = false, Description = "Use the spotdl file to get metadata")]
    public bool UseSpotdl { get; init; } = false;

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        if (!ValidatePlaylist(PlaylistName, console))
            return;

        var playlistPath = Path.Combine(_fileService.GetPlaylistsPath(), PlaylistName);
        await _mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}