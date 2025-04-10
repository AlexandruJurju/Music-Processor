using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.UseCases.LogMissing;

namespace MusicProcessor.CLI.MenuCommands;

[Command("log-missing", Description = "Log missing songs that don't have metadata")]
public class LogMissingMenuCommand(ISender sender) : BaseMenuCommand(sender)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _sender.Send(new LogMissingQuery());
    }
}
