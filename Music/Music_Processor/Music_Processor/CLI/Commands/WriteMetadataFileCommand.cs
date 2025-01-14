using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class WriteMetadataFileCommand : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly ILogger<WriteMetadataFileCommand> _logger;
    private readonly IMetadataService _metadataService;

    public WriteMetadataFileCommand(ILogger<WriteMetadataFileCommand> logger, IFileService fileService, IMetadataService metadataService)
    {
        _logger = logger;
        _fileService = fileService;
        _metadataService = metadataService;
    }

    public string Name => "Write metadata file";

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
        var playlistFolders = availablePlaylists.Select(path => Path.GetFileName(path)).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }

        try
        {
            var folderPath = Path.Combine(_fileService.GetPlaylistsDirectory(), playlistName);
            var playlistMetadata =  _metadataService.GetPlaylistSongsMetadata(folderPath);
            await _metadataService.SaveMetadataToJsonAsync(playlistMetadata, $"{folderPath}.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write metadata for playlist");
            Console.WriteLine($"\nError: {ex.Message}");
        }
    }
}