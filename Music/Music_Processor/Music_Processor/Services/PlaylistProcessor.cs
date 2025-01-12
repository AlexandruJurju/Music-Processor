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
            var songName = Path.GetFileNameWithoutExtension(song);
            var cleanedSongName = _spotdlMetadataLoader.CleanKeyName(songName);

            if (!spotdlMetadata.TryGetValue(cleanedSongName, out var songSpotDLMetadata))
            {
                // _logger.LogInformation($"Song {songName} was not found in Spotdl metadata file.");
                SongsWithoutMetadata.Add(songName);
                continue;
            }

            // Move genres to styles - make it so i can use the process song for external metadata
            songSpotDLMetadata.Styles.Clear();
            songSpotDLMetadata.Styles.AddRange(songSpotDLMetadata.Genres);
            songSpotDLMetadata.Genres.Clear();

            ProcessSongMetadata(songSpotDLMetadata, styleMappings, stylesToRemove);

            if (!songSpotDLMetadata.Styles.Any())
            {
                SongsWithoutStyles.Add(songName);
            }
        }

        PrintProcessingResults();
    }

    private void ProcessSongMetadata(AudioMetadata songMetadata, Dictionary<string, List<string>> styleMappings, List<string> stylesToRemove)
    {
        foreach (var mapping in styleMappings)
        {
            foreach (var style in songMetadata.Styles.ToList())
            {
                if (mapping.Value.Contains(style))
                {
                    // Remove the original style if it's the same as the key
                    if (mapping.Key == style)
                    {
                        songMetadata.Styles.Remove(style);
                    }

                    // remove the style if it's a style that needs to be removed
                    if (stylesToRemove.Contains(style))
                    {
                        songMetadata.Styles.Remove(style);
                    }

                    // Add the mapped style if it's not already present
                    if (!songMetadata.Genres.Contains(mapping.Key))
                    {
                        songMetadata.Genres.Add(mapping.Key);
                    }

                    // _logger.LogInformation($"Mapped style {style} to {mapping.Key} for song {songName}");
                }
            }
        }
    }

    private void PrintProcessingResults()
    {
        Console.WriteLine("Songs without metadata:");
        foreach (var song in SongsWithoutMetadata)
        {
            Console.WriteLine(song);
        }
    }
}