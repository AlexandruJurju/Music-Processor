using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;

namespace MusicProcessor.CLI.MenuCommands;

[Command("write-db", Description = "Write a library metadata a database")]
public class WriteLibraryToDb(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    // todo: parameter to use spotdl file or not
    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public string PlaylistName { get; init; } = "";

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        // todo: fix, just send the name
        var playlistPath = Path.Combine(FileService.GetPlaylistsPath(), PlaylistName);
        await Mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}