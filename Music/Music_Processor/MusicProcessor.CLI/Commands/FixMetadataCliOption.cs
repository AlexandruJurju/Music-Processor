using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.CLI.Commands;

[Command("fix-metadata", Description = "Fix metadata for a playlist")]
public class FixMetadataCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public string PlaylistName { get; init; } = "";

    public void Validate()
    {
        if (!FileService.GetAllPlaylistsNames().Contains(PlaylistName))
            throw new CommandException($"Invalid playlist: {PlaylistName}");
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        var playlistPath = GetPlaylistPath(PlaylistName);
        await Mediator.Send(new Application.UseCases.FixMetadata.FixMetadataCommand(playlistPath));
    }
}