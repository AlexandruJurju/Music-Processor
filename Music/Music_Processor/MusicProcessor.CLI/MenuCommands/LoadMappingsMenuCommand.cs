using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.UseCases.LoadMappings;

namespace MusicProcessor.CLI.MenuCommands;

[Command("load-mappings", Description = "Loads mapping files from the current directory")]
public class LoadMappingsMenuCommand(IFileService fileService, IMediator mediator) : BaseMenuCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await Mediator.Send(new LoadMappingsCommand());
    }
}