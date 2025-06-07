using CliFx.Attributes;
using CliFx.Infrastructure;
using Mediator;
using MusicProcessor.Application.Songs.LogMissing;
using ICommand = CliFx.ICommand;


namespace MusicProcessor.Console.MenuCommands;

[Command("log-missing")]
public class LogMissingSongsCliCommand(
    ISender sender
) : ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var command = new LogMissingCommand();
        await sender.Send(command);
    }
}
