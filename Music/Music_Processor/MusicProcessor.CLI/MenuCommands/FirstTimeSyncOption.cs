using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.SyncCommand;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuCommands;

public class FirstTimeSyncOption : IMenuOption
{
    private readonly IFileService _fileService;
    private readonly ISpotDLService _spotdlService;
    private readonly IMediator _mediator;

    public FirstTimeSyncOption(ISpotDLService spotdlService, IFileService fileService, IMediator mediator)
    {
        _spotdlService = spotdlService;
        _fileService = fileService;
        _mediator = mediator;
    }

    public string Name => "SPOTDL: First Time Sync";

    public async Task ExecuteAsync()
    {
        Console.Write("\nEnter Spotify playlist URL: ");
        var playlistUrl = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(playlistUrl))
        {
            Console.WriteLine("Please provide a valid Spotify playlist URL.");
            return;
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return;
        }

        var playlistDirPath = Path.Combine(_fileService.GetPlaylistsPath(), playlistName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nSpotDL Started");

        await foreach (var output in _spotdlService.NewSyncAsync(playlistUrl, playlistDirPath))
        {
            Console.ForegroundColor = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(output.Data);
            Console.ResetColor();
        }

        Console.WriteLine("\nSpot DL Finished");
        Console.WriteLine("\nAdding new songs to db");
        await _mediator.Send(new SyncDbCommand(playlistDirPath));
    }
}