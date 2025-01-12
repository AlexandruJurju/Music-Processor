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
        // Create HashSet from stylesToRemove for O(1) lookups
        var stylesToRemoveSet = new HashSet<string>(stylesToRemove);

        // Track which styles were mapped
        var mappedStyles = new HashSet<string>();

        // Process each style only once
        foreach (var style in songMetadata.Styles.ToList())
        {
            bool styleWasMapped = false;

            // Check if any mapping contains this style
            foreach (var mapping in styleMappings)
            {
                var mappingGenre = mapping.Key;
                if (mapping.Value.Contains(style))
                {
                    styleWasMapped = true;

                    // Remove if it's the same as the genre or in stylesToRemove
                    if (mappingGenre == style || stylesToRemoveSet.Contains(style))
                    {
                        songMetadata.Styles.Remove(style);
                    }

                    // Add mapped genre if not present
                    if (!songMetadata.Genres.Contains(mappingGenre))
                    {
                        songMetadata.Genres.Add(mappingGenre);
                    }

                    break; // Style is mapped, no need to check other mappings
                }
            }

            // If style wasn't mapped, add it to unmapped styles
            if (!styleWasMapped && !stylesToRemoveSet.Contains(style))
            {
                UnmappedStyles.Add(style);
            }
        }
    }

    private void PrintProcessingResults()
    {
        Console.WriteLine("\nSongs without metadata:");
        foreach (var song in SongsWithoutMetadata)
        {
            Console.WriteLine(song);
        }

        Console.WriteLine("\nSongs without styles:");
        foreach (var song in SongsWithoutStyles)
        {
            Console.WriteLine(song);
        }

        Console.WriteLine("\nUnmapped styles found:");
        foreach (var style in UnmappedStyles)
        {
            Console.WriteLine(style);
        }
    }
}