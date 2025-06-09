using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.Songs.ExportMissingSongsMetadata;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.Console.MenuCommands;

[Command("export-missing-songs-metadata")]
public class ExportMissingSongsMetadataCliCommand(
    ISender sender
) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new ExportMissingSongsMetadataCommand();
        
        await sender.Send(command);
    }
}
