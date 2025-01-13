using System.Text.Json;
using Microsoft.Extensions.Logging;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;
using Music_Processor.Services.SpotDLMetadataLoader;

namespace Music_Processor.Services;

public class PlaylistProcessor(
    ILogger<PlaylistProcessor> logger,
    IFileService fileService,
    SpotdlMetadataLoader spotdlMetadataLoader,
    IConfigService configService,
    MetadataHandlesFactory metadataHandlesFactory)
    : IPlaylistProcessor
{
    private readonly ILogger<PlaylistProcessor> _logger = logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HashSet<string> UnmappedStyles = new();
    private readonly HashSet<string> SongsWithoutStyles = new();
    private readonly HashSet<string> SongsWithoutMetadata = new();

    public void FixPlaylistGenresUsingSpotdlMetadata(string playlistPath)
    {
        var spotdlMetadata = spotdlMetadataLoader.LoadSpotDLMetadata(playlistPath);
        var playlistSongs = fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = configService.LoadStyleMappingFile();
        var stylesToRemove = configService.LoadStylesToRemove();

        foreach (var song in playlistSongs)
        {
            var songName = Path.GetFileNameWithoutExtension(song);

            var cleanedSongName = spotdlMetadataLoader.CleanKeyName(songName);

            if (!spotdlMetadata.TryGetValue(cleanedSongName, out var songSpotDLMetadata))
            {
                // _logger.LogInformation($"Song {songName} was not found in Spotdl metadata file.");
                SongsWithoutMetadata.Add(songName);
                continue;
            }

            PlaceGenresInStyles(songSpotDLMetadata);

            ProcessSongMetadata(songSpotDLMetadata, styleMappings, stylesToRemove);

            if (!songSpotDLMetadata.Styles.Any())
            {
                SongsWithoutStyles.Add(songName);
                continue;
            }

            var metadataHandler = metadataHandlesFactory.GetHandler(song);
            metadataHandler.WriteMetadata(song, songSpotDLMetadata);
        }

        PrintProcessingResults();
    }

    private void PlaceGenresInStyles(AudioMetadata songSpotDLMetadata)
    {
        // Move genres to styles - make it so i can use the process song for external metadata
        songSpotDLMetadata.Styles.Clear();
        songSpotDLMetadata.Styles.AddRange(songSpotDLMetadata.Genres);
        songSpotDLMetadata.Genres.Clear();
    }

    public void FixPlaylistGenresUsingCustomMetadata(string playlistPath, string metadataPath)
    {
        string jsonContent = File.ReadAllText(metadataPath);
        List<AudioMetadata> customMetadata = JsonSerializer.Deserialize<List<AudioMetadata>>(jsonContent, _jsonOptions)!;

        // dictionary using the song file path as key
        var metadataByPath = customMetadata.ToDictionary(m => m.FilePath, m => m);
        var songs = fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = configService.LoadStyleMappingFile();
        var stylesToRemove = configService.LoadStylesToRemove();

        foreach (var song in songs)
        {
            if (metadataByPath.TryGetValue(song, out var songMetadata))
            {
                PlaceGenresInStyles(songMetadata);

                ProcessSongMetadata(songMetadata, styleMappings, stylesToRemove);

                var metadataHandler = metadataHandlesFactory.GetHandler(song);
                metadataHandler.WriteMetadata(song, songMetadata);
            }
            else
            {
                SongsWithoutMetadata.Add(Path.GetFileNameWithoutExtension(song));
            }
        }

        PrintProcessingResults();
    }

    private void ProcessSongMetadata(AudioMetadata songMetadata, Dictionary<string, List<string>> styleMappings, List<string> stylesToRemove)
    {
        // Create HashSet from stylesToRemove for O(1) lookups
        var stylesToRemoveSet = new HashSet<string>(stylesToRemove);

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