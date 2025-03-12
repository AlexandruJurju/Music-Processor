using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuCommands;

[Command("sync-update", Description = "Update sync with existing playlist")]
public class UpdateSyncMenuCommand : BaseMenuCommand
{
    private readonly ISpotDLService _spotDlService;

    public UpdateSyncMenuCommand(IFileService fileService, IMediator mediator, ISpotDLService spotDlService) : base(fileService, mediator)
    {
        _spotDlService = spotDlService;
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "The playlist name")]
    public string PlaylistName { get; init; } = String.Empty;

    public void Validate()
    {
        if (!_fileService.GetAllPlaylistsNames().Contains(PlaylistName))
            throw new CommandException($"Invalid playlist: {PlaylistName}");
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        if (!ValidatePlaylist(PlaylistName, console))
            return;

        var playlistPath = GetPlaylistPath(PlaylistName);

        console.Output.WriteLine("\nSpotDL Started", ConsoleColor.Green);

        await foreach (var output in _spotDlService.UpdateSyncAsync(playlistPath))
        {
            var color = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
            console.Output.WriteLine(output.Data, color);
        }

        await console.Output.WriteLineAsync("\nSpot DL Finished");
        await console.Output.WriteLineAsync("\nAdding new songs to db");
        await _mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}