using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

namespace MusicProcessor.CLI.MenuCommands;

[Command("spotdl-read-metadata", Description = "Read the infos of the songs and insert them to the db")]
public class ReadSpotDLMeta : BaseMenuCommand
{
    public ReadSpotDLMeta(
        IMediator mediator
    ) : base(mediator)
    {
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public required string PlaylistName { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _mediator.Send(new ReadSpotdlMetadataCommand(PlaylistName));
    }
}