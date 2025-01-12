using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly ILogger<PlaylistProcessor> _logger;
    private readonly IFileService _fileService;
    private readonly IMetadataService _metadataService;

    private HashSet<string> UnmappedStyles;
    private HashSet<string> SongsWithoutStyles;
    private HashSet<string> SongsWithoutMetadata;

    public PlaylistProcessor(ILogger<PlaylistProcessor> logger, IFileService fileService, IMetadataService metadataService)
    {
        _logger = logger;
        _fileService = fileService;
        _metadataService = metadataService;
        UnmappedStyles = new HashSet<string>();
        SongsWithoutStyles = new HashSet<string>();
        SongsWithoutMetadata = new HashSet<string>();
    }

    public Dictionary<string, AudioMetadata> LoadSpotDLMetadata(string playlistPath)
    {
        Dictionary<string, AudioMetadata> playlistMetadata = new Dictionary<string, AudioMetadata>();

        var spotdlFile = _fileService.GetSpotDLFileInFolder(playlistPath);
        if (spotdlFile is null)
        {
            _logger.LogError("Spotdl file could not be found");
            throw new FileNotFoundException("Spotdl file not found");
        }


        return playlistMetadata;
    }
}