using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.Songs.ExportMetadata;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.Console.MenuCommands;

[Command("export", Description = "Export metadata to a json file")]
public class ExportMetadataCliCommand(
    ISender sender
) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new ExportMetadataCommand();
        await sender.Send(command);
    }
}
