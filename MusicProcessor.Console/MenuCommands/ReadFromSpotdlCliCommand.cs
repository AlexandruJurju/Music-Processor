using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Application.Songs.ReadSongsFromJson;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.Console.MenuCommands;

[Command("read-from-spotdl")]
public class ReadFromSpotdlCliCommand(
    ISender sender
) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new ReadMetadataFromFileCommand();
        await sender.Send(command);
    }
}
