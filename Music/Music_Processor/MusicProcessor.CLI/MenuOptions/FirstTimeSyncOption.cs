using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.CLI.MenuOptions.Abstractions;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuOptions;

public class FirstTimeSyncOption(
    ISpotDLService spotdlService,
    IFileService fileService,
    IMediator mediator
) : BaseMenuOption(fileService, mediator)
{
    public override string Name => "SPOTDL: First Time Sync";

    public override async Task ExecuteAsync()
    {
        Console.Write("\nEnter Spotify playlist URL: ");
        var playlistUrl = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(playlistUrl))
        {
            Console.WriteLine("Please provide a valid Spotify playlist URL.");
            return;
        }

        var playlistName = GetValidatedPlaylistName(validateExistence: false);
        if (playlistName == null) return;

        var playlistPath = GetPlaylistPath(playlistName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nSpotDL Started");

        await foreach (var output in spotdlService.NewSyncAsync(playlistUrl, playlistPath))
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