using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Music_Processor.Constants;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class FixGenresSpotDLCommand : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<FixGenresSpotDLCommand> _logger;
    private readonly IPlaylistProcessor _playlistProcessor;

    public FixGenresSpotDLCommand(ILogger<FixGenresSpotDLCommand> logger, IFileService fileService, IPlaylistProcessor playlistProcessor)
    {
        _logger = logger;
        _fileService = fileService;
        _playlistProcessor = playlistProcessor;
    }

    public string Name => "Fix Genres SpotDL";

    public async Task ExecuteAsync()
    {
        var baseDirectory = AppPaths.PlaylistsDirectory;
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
        var playlistFolders = availablePlaylists.Select(Path.GetFileName).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }
        
        var stopwatch = Stopwatch.StartNew();
        var playlistPath = Path.Combine(AppPaths.PlaylistsDirectory, playlistName);
        await _playlistProcessor.FixPlaylistGenresUsingSpotdlMetadataAsync(playlistPath);
        Console.WriteLine($"Finished in {stopwatch.Elapsed.Milliseconds}ms");
    }
}