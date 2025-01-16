using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.CLI.MenuCommands;

public class FirstTimeSyncCommand : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<FirstTimeSyncCommand> _logger;
    private readonly ISpotDLService _spotdlService;

    public FirstTimeSyncCommand(ILogger<FirstTimeSyncCommand> logger, ISpotDLService spotdlService, IFileService fileService)
    {
        _logger = logger;
        _spotdlService = spotdlService;
        _fileService = fileService;
    }

    public string Name => "First Time Sync";

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

        try
        {
            Console.WriteLine("Calling SpotDL...");
            await _spotdlService.NewSyncAsync(playlistUrl, playlistName, _fileService.GetPlaylistsPath());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync playlist");
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}