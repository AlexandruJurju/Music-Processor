using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

namespace MusicProcessor.CLI.Commands;

[Command("write-mapping", Description = "Write current mapping file to json")]
public class WriteStyleMappingCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await Mediator.Send(new WriteStyleMappingConfigCommand());
    }
}