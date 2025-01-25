using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

namespace MusicProcessor.CLI.MenuCommands;

[Command("write-db", Description = "Write a library metadata to the database")]
public class WriteLibraryToDbMenuCommand(IFileService fileService, IMediator mediator) : BaseMenuCommand(fileService, mediator)
{
    // todo: parameter to use spotdl file or not
    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public string PlaylistName { get; init; } = "";

    [CommandOption("use-spotdl", 's', IsRequired = false, Description = "Use the spotdl file to get metadata")]
    public bool UseSpotdl { get; init; } = false;

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        if (!ValidatePlaylist(PlaylistName, console))
        {
            return;
        }

        // todo: fix, just send the name
        var playlistPath = Path.Combine(FileService.GetPlaylistsPath(), PlaylistName);
        await Mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}