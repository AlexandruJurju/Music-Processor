using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class UpdateSyncCommand : IMenuCommand
{
    private readonly ILogger<UpdateSyncCommand> _logger;
    private readonly ISpotDLService _spotdlService;
    private readonly IFileService _fileService;


    public UpdateSyncCommand(ILogger<UpdateSyncCommand> logger, ISpotDLService spotdlService, IFileService fileService)
    {
        _logger = logger;
        _spotdlService = spotdlService;
        _fileService = fileService;
    }

    public string Name => "Update Sync";
    public int MenuNumber => MenuChoices.UpdateSync;

    public async Task ExecuteAsync()
    {
        string baseDirectory = _fileService.GetPlaylistsDirectory();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);

        foreach (var playlist in availablePlaylists)
        {
            string folderName = Path.GetFileName(playlist);
            Console.WriteLine(folderName);
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();
        if (String.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return;
        }

        // check if entered playlist name exists in available playlists
        var playlistFolders = availablePlaylists.Select(path => Path.GetFileName(path)).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }

        // check if sync file exists
        var syncFile = Path.Combine(_fileService.GetPlaylistsDirectory(), playlistName, $"{playlistName}.spotdl");
        if (string.IsNullOrEmpty(syncFile))
        {
            Console.WriteLine("No spotdl file found.");
        }

        try
        {
            Console.WriteLine("Calling SpotDL...");
            await _spotdlService.UpdateSyncAsync(playlistName, _fileService.GetPlaylistsDirectory());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync playlist");
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}