using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class FirstTimeSyncCommand : IMenuCommand
{
    private readonly ILogger<FirstTimeSyncCommand> _logger;
    private readonly ISpotDLService _spotdlService;
    private readonly IFileService _fileService;

    public FirstTimeSyncCommand(ILogger<FirstTimeSyncCommand> logger, ISpotDLService spotdlService, IFileService fileService)
    {
        _logger = logger;
        _spotdlService = spotdlService;
        _fileService = fileService;
    }

    public string Name => "First Time Sync";
    public int MenuNumber => MenuChoices.FirstTimeSync;

    public async Task ExecuteAsync()
    {
        Console.Write("\nEnter Spotify playlist URL: ");
        var playlistUrl = Console.ReadLine()?.Trim();

        if (String.IsNullOrEmpty(playlistUrl))
        {
            Console.WriteLine("Please provide a valid Spotify playlist URL.");
            return;
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();

        if (String.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return;
        }

        try
        {
            Console.WriteLine("Calling SpotDL...");
            await _spotdlService.NewSyncAsync(playlistUrl, playlistName, _fileService.GetPlaylistsDirectory());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync playlist");
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}