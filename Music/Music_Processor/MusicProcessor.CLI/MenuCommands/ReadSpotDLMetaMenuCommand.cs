using CliFx.Attributes;
using CliFx.Infrastructure;
using MusicProcessor.Application.UseCases.ReadSpotdlMetadata;
using Wolverine;

namespace MusicProcessor.CLI.MenuCommands;

[Command("spotdl-read-metadata", Description = "Read the infos of the songs and insert them to the db")]
public class ReadSpotDLMetaMenuCommand : BaseMenuCommand
{
    public ReadSpotDLMetaMenuCommand(IMessageBus messageBus) : base(messageBus)
    {
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public required string PlaylistName { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _messageBus.InvokeAsync(new ReadSpotdlMetadataCommand(PlaylistName));
    }
}