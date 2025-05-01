using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.UseCases.CommitChanges;

namespace MusicProcessor.CLI.MenuCommands;

[Command("commit", Description = "Commit changes to the library")]
public class CommitChangesMenuCommand(ISender sender) : BaseMenuCommand(sender)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _sender.Send(new CommitChangesCommand());
    }
}
