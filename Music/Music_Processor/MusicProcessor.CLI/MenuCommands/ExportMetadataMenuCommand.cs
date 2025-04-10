using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.UseCases.ExportMetadata;

namespace MusicProcessor.CLI.MenuCommands;

[Command("export-metadata")]
public class ExportMetadataMenuCommand(ISender sender) : BaseMenuCommand(sender)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _sender.Send(new ExportMetadataCommand());
    }
}
