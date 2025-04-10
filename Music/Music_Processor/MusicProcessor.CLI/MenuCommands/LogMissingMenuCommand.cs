using CliFx.Attributes;
using CliFx.Infrastructure;
using MusicProcessor.Application.UseCases.LogMissing;
using Wolverine;

namespace MusicProcessor.CLI.MenuCommands;

[Command("log-missing", Description = "Log missing songs that don't have metadata")]
public class LogMissingMenuCommand : BaseMenuCommand
{
    public LogMissingMenuCommand(IMessageBus messageBus) : base(messageBus)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _messageBus.SendAsync(new LogMissingQuery());
    }
}