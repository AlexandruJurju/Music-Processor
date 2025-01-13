using System.Text.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;
using Music_Processor.Services.SpotDLMetadataLoader;

namespace Music_Processor.Services;

public class PlaylistProcessor(
    IFileService fileService,
    SpotdlMetadataLoader spotdlMetadataLoader,
    IConfigService configService,
    MetadataHandlesFactory metadataHandlesFactory)
    : IPlaylistProcessor
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly ConcurrentDictionary<string, byte> UnmappedStyles = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutStyles = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutMetadata = new();

    public void FixPlaylistGenresUsingSpotdlMetadata(string playlistPath)
    {
        var spotdlMetadata = spotdlMetadataLoader.LoadSpotDLMetadata(playlistPath);
        var playlistSongs = fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = configService.LoadStyleMappingFile();
        var stylesToRemove = configService.LoadStylesToRemove();
        
        Parallel.ForEach(
            playlistSongs,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            song =>
            {
                var songName = Path.GetFileNameWithoutExtension(song);
                var cleanedSongName = spotdlMetadataLoader.CleanKeyName(songName);

                if (!spotdlMetadata.TryGetValue(cleanedSongName, out var songSpotDLMetadata))
                {
                    SongsWithoutMetadata.TryAdd(songName, 1);
                    return;
                }

                PlaceGenresInStyles(songSpotDLMetadata);
                ProcessSongMetadata(songSpotDLMetadata, styleMappings, stylesToRemove);

                if (!songSpotDLMetadata.Styles.Any())
                {
                    SongsWithoutStyles.TryAdd(songName, 1);
                    return;
                }

                var metadataHandler = metadataHandlesFactory.GetHandler(song);
                metadataHandler.WriteMetadata(song, songSpotDLMetadata);
            });
        
        PrintProcessingResults();
    }

    private void PlaceGenresInStyles(AudioMetadata songSpotDLMetadata)
    {
        songSpotDLMetadata.Styles.Clear();
        songSpotDLMetadata.Styles.AddRange(songSpotDLMetadata.Genres);
        songSpotDLMetadata.Genres.Clear();
    }

    public void FixPlaylistGenresUsingCustomMetadata(string playlistPath, string metadataPath)
    {
        string jsonContent = File.ReadAllText(metadataPath);
        List<AudioMetadata> customMetadata = JsonSerializer.Deserialize<List<AudioMetadata>>(jsonContent, _jsonOptions)!;

        var metadataByPath = customMetadata.ToDictionary(m => m.FilePath, m => m);
        var songs = fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = configService.LoadStyleMappingFile();
        var stylesToRemove = configService.LoadStylesToRemove();

        Parallel.ForEach(
            songs,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            song =>
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
                    SongsWithoutMetadata.TryAdd(Path.GetFileNameWithoutExtension(song), 1);
                }
            });

        PrintProcessingResults();
    }

    private void ProcessSongMetadata(AudioMetadata songMetadata, Dictionary<string, List<string>> styleMappings, List<string> stylesToRemove)
    {
        var stylesToRemoveSet = new HashSet<string>(stylesToRemove);

        foreach (var style in songMetadata.Styles.ToList())
        {
            bool styleWasMapped = false;

            foreach (var mapping in styleMappings)
            {
                var mappingGenre = mapping.Key;
                if (mapping.Value.Contains(style))
                {
                    styleWasMapped = true;

                    if (mappingGenre == style || stylesToRemoveSet.Contains(style))
                    {
                        songMetadata.Styles.Remove(style);
                    }

                    if (!songMetadata.Genres.Contains(mappingGenre))
                    {
                        songMetadata.Genres.Add(mappingGenre);
                    }

                    break;
                }
            }

            if (!styleWasMapped && !stylesToRemoveSet.Contains(style))
            {
                UnmappedStyles.TryAdd(style, 1);
            }
        }
    }

    private void PrintProcessingResults()
    {
        Console.WriteLine("\nSongs without metadata:");
        foreach (var song in SongsWithoutMetadata.Keys)
        {
            Console.WriteLine(song);
        }

        Console.WriteLine("\nSongs without styles:");
        foreach (var song in SongsWithoutStyles.Keys)
        {
            Console.WriteLine(song);
        }

        Console.WriteLine("\nUnmapped styles found:");
        foreach (var style in UnmappedStyles.Keys)
        {
            Console.WriteLine(style);
        }
    }
}