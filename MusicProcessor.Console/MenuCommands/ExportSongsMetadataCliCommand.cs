using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.PhysicalSongs.ExportMetadata;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.Console.MenuCommands;

[Command("export-songs-metadata")]
public class ExportSongsMetadataCliCommand (
    ISender sender
    ) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new ExportMetadataCommand();
        
        await sender.Send(command);
    }
}
