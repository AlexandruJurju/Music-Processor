using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.CLI.MenuCommands;

public class UpdateSyncCommand : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<UpdateSyncCommand> _logger;
    private readonly ISpotDLService _spotdlService;


    public UpdateSyncCommand(ILogger<UpdateSyncCommand> logger, ISpotDLService spotdlService, IFileService fileService)
    {
        _logger = logger;
        _spotdlService = spotdlService;
        _fileService = fileService;
    }

    public string Name => "Update Sync";

    public async Task ExecuteAsync()
    {
        var baseDirectory = _fileService.GetPlaylistsPath();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);

        foreach (var playlist in availablePlaylists)
        {
            var folderName = Path.GetFileName(playlist);
            Console.WriteLine(folderName);
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(playlistName))
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
        var syncFile = Path.Combine(_fileService.GetPlaylistsPath(), playlistName, $"{playlistName}.spotdl");
        if (string.IsNullOrEmpty(syncFile))
        {
            Console.WriteLine("No spotdl file found.");
        }

        try
        {
            Console.WriteLine("Calling SpotDL...");
            await _spotdlService.UpdateSyncAsync(playlistName, _fileService.GetPlaylistsPath());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync playlist");
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}