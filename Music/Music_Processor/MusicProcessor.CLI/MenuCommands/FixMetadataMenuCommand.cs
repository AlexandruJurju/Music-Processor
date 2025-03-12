using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.FixMetadata;

namespace MusicProcessor.CLI.MenuCommands;

[Command("fix-metadata", Description = "Fix metadata for a playlist")]
public class FixMetadataMenuMenuCommand : BaseMenuCommand
{
    public FixMetadataMenuMenuCommand(IFileService fileService, IMediator mediator) : base(fileService, mediator)
    {
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public required string PlaylistName { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        if (!ValidatePlaylist(PlaylistName, console)) return;

        var playlistPath = GetPlaylistPath(PlaylistName);
        await _mediator.Send(new FixMetadataCommand(playlistPath));
    }
}