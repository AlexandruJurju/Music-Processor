using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly ILogger<PlaylistProcessor> _logger;
    private readonly IFileService _fileService;

    private HashSet<string> UnmappedStyles;
    private HashSet<string> SongsWithoutStyles;
    private HashSet<string> SongsWithoutMetadata;

    public PlaylistProcessor(ILogger<PlaylistProcessor> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
        UnmappedStyles = new HashSet<string>();
        SongsWithoutStyles = new HashSet<string>();
        SongsWithoutMetadata = new HashSet<string>();
    }

    public void ProcessPlaylist(string playlistPath)
    {
    }

    public Dictionary<string, SongMetadata> LoadPlaylistMetadata(string playlistPath)
    {
        Dictionary<string, SongMetadata> playlistMetadata = new Dictionary<string, SongMetadata>();

        string metadataFile = _fileService.GetMetadataStorageFileForPlaylist(playlistPath);

        return playlistMetadata;
    }
}