using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.LoadMappings;

namespace MusicProcessor.CLI.Commands;

[Command("load-mappings", Description = "Loads mapping files from the current directory")]
public class LoadMappingsOption(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await Mediator.Send(new LoadMappingsCommand());
    }
}