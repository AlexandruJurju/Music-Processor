using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI.Commands;

public class FirstTimeSyncCommand : ICommand
{
    private readonly ILogger<FirstTimeSyncCommand> _logger;

    public FirstTimeSyncCommand(ILogger<FirstTimeSyncCommand> logger)
    {
        _logger = logger;
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
        
    }
}