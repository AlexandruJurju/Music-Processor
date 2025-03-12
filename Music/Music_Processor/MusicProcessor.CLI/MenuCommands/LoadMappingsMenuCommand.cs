using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.LoadMappings;

namespace MusicProcessor.CLI.MenuCommands;

[Command("load-mappings", Description = "Loads mapping files from the current directory")]
public class LoadMappingsMenuCommand : BaseMenuCommand
{
    public LoadMappingsMenuCommand(IFileService fileService, IMediator mediator) : base(fileService, mediator)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _mediator.Send(new LoadMappingsCommand());
    }
}