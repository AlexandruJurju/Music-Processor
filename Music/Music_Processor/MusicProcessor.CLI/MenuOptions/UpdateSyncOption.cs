using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.CLI.MenuOptions.Abstractions;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuOptions;

public class UpdateSyncOption(ISpotDLService spotDlService, IFileService fileService, IMediator mediator) : BaseMenuOption(fileService, mediator)
{
    public override string Name => "SPOTDL: Update Sync";

    public override async Task ExecuteAsync()
    {
        var playlistName = GetValidatedPlaylistName();
        if (playlistName == null) return;

        var playlistPath = GetPlaylistPath(playlistName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nSpotDL Started");

        await foreach (var output in spotDlService.UpdateSyncAsync(playlistPath))
        {
            Console.ForegroundColor = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(output.Data);
            Console.ResetColor();
        }

        Console.WriteLine("\nSpot DL Finished");
        Console.WriteLine("\nAdding new songs to db");
        await _mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}