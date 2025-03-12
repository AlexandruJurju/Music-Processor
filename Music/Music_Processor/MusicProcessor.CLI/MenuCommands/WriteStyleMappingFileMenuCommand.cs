using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

namespace MusicProcessor.CLI.MenuCommands;

[Command("write-mappings", Description = "Write current style mappings file to json")]
public class WriteStyleMappingsMenuCommand : BaseMenuCommand
{
    public WriteStyleMappingsMenuCommand(IFileService fileService, IMediator mediator) : base(fileService, mediator)
    {
    }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await _mediator.Send(new WriteStyleMappingsCommand());
    }
}