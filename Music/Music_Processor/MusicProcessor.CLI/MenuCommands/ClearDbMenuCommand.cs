using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.ClearPersistence;

namespace MusicProcessor.CLI.MenuCommands;

[Command("clear-db", Description = "Delete the data stored in the database")]
public class ClearDbMenuCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override async ValueTask ExecuteAsync(IConsole console)
    {
        await Mediator.Send(new ClearDbCommand());
    }
}