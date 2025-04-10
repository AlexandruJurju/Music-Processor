using CliFx.Attributes;
using CliFx.Infrastructure;
using MusicProcessor.Application.UseCases.CommitChanges;
using Wolverine;

namespace MusicProcessor.CLI.MenuCommands;

[Command("commit", Description = "Commit changes to the library")]
public class CommitChangesMenuCommand : BaseMenuCommand
{
    public CommitChangesMenuCommand(IMessageBus messageBus) : base(messageBus)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
    }
}