using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.Songs.ReadSongsFromJson;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.Console.MenuCommands;

[Command("read-from-json")]
public class ReadFromJson(
    ISender sender
) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new ReadSongsFromJsonCommand();

        await sender.Send(command);
    }
}