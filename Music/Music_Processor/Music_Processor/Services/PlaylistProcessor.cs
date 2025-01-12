using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;
using Music_Processor.Model;
using Music_Processor.Services.SpotDLMetadataLoader;

namespace Music_Processor.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly ILogger<PlaylistProcessor> _logger;
    private readonly IFileService _fileService;
    private readonly IMetadataService _metadataService;
    private readonly SpotdlMetadataLoader _spotdlMetadataLoader;
    private readonly IConfigService _configService;

    private HashSet<string> UnmappedStyles;
    private HashSet<string> SongsWithoutStyles;
    private HashSet<string> SongsWithoutMetadata;

    public PlaylistProcessor(ILogger<PlaylistProcessor> logger, IFileService fileService, IMetadataService metadataService, SpotdlMetadataLoader spotdlMetadataLoader, IConfigService configService)
    {
        UnmappedStyles = new HashSet<string>();
        SongsWithoutStyles = new HashSet<string>();
        SongsWithoutMetadata = new HashSet<string>();

        _logger = logger;
        _fileService = fileService;
        _metadataService = metadataService;
        _spotdlMetadataLoader = spotdlMetadataLoader;
        _configService = configService;
    }

    public void FixPlaylistGenresUsingSpotdlMetadata(string playlistPath)
    {
        var spotdlMetadata = _spotdlMetadataLoader.LoadSpotDLMetadata(playlistPath);
        var playlistSongs = _fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = _configService.LoadStyleMappingFile();
        var stylesToRemove = _configService.LoadStylesToRemove();

        foreach (var song in playlistSongs)
        {
            ProcessSong(song);
        }
    }

    private void ProcessSong(string songPath)
    {
        throw new NotImplementedException();
    }
}