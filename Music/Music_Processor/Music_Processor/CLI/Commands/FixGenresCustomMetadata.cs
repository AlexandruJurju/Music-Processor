using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class FixGenresCustomMetadata : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<FixGenresCustomMetadata> _logger;
    private readonly IPlaylistProcessor _playlistProcessor;

    public FixGenresCustomMetadata(ILogger<FixGenresCustomMetadata> logger, IFileService fileService, IPlaylistProcessor playlistProcessor)
    {
        _logger = logger;
        _fileService = fileService;
        _playlistProcessor = playlistProcessor;
    }

    public string Name => "Fix Genres using custom metadata";

    public async Task ExecuteAsync()
    {
        var baseDirectory = _fileService.GetPlaylistsDirectory();
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

        var metadataFile = Path.Combine(_fileService.GetPlaylistsDirectory(), playlistName + ".json");
        if (!File.Exists(metadataFile))
        {
            Console.WriteLine("Metadata file could not be found.");
            return;
        }

        var playlistPath = Path.Combine(_fileService.GetPlaylistsDirectory(), playlistName);
        _playlistProcessor.FixPlaylistGenresUsingCustomMetadata(playlistPath, metadataFile);
    }
}