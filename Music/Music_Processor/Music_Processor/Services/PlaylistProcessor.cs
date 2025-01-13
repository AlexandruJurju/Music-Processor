using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;
using Music_Processor.Services.SpotDLMetadataLoader;

namespace Music_Processor.Services;

public class PlaylistProcessor: IPlaylistProcessor
{
    private readonly IFileService _fileService;
    private readonly SpotdlMetadataLoader _spotdlMetadataLoader;
    private readonly IConfigService _configService;
    private readonly MetadataHandlesFactory _metadataHandlesFactory;

    public PlaylistProcessor(
        IFileService fileService,
        SpotdlMetadataLoader spotdlMetadataLoader,
        IConfigService configService,
        MetadataHandlesFactory metadataHandlesFactory)
    {
        _fileService = fileService;
        _spotdlMetadataLoader = spotdlMetadataLoader;
        _configService = configService;
        _metadataHandlesFactory = metadataHandlesFactory;
    }
    
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ConcurrentDictionary<string, byte> SongsWithoutMetadata = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutStyles = new();

    private readonly ConcurrentDictionary<string, byte> UnmappedStyles = new();

    public void FixPlaylistGenresUsingSpotdlMetadata(string playlistPath)
    {
        var spotdlMetadata = _spotdlMetadataLoader.LoadSpotDLMetadata(playlistPath);
        var playlistSongs = _fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = _configService.LoadStyleMappingFile();
        var stylesToRemove = _configService.LoadStylesToRemove();

        var stopwatch = Stopwatch.StartNew();

        Parallel.ForEach(
            playlistSongs,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            song =>
            {
                var songName = Path.GetFileNameWithoutExtension(song);
                var cleanedSongName = _spotdlMetadataLoader.CleanKeyName(songName);

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

                var metadataHandler = _metadataHandlesFactory.GetHandler(song);
                metadataHandler.WriteMetadata(song, songSpotDLMetadata);
            });

        Console.WriteLine($"Finished in {stopwatch.ElapsedMilliseconds}ms");

        // PrintProcessingResults();
    }

    public void FixPlaylistGenresUsingCustomMetadata(string playlistPath, string metadataPath)
    {
        var jsonContent = File.ReadAllText(metadataPath);
        List<AudioMetadata> customMetadata = JsonSerializer.Deserialize<List<AudioMetadata>>(jsonContent, _jsonOptions)!;

        var metadataByPath = customMetadata.ToDictionary(m => m.FilePath, m => m);
        var songs = _fileService.GetAllAudioFilesInFolder(playlistPath);
        var styleMappings = _configService.LoadStyleMappingFile();
        var stylesToRemove = _configService.LoadStylesToRemove();

        Parallel.ForEach(
            songs,
            new ParallelOptions { MaxDegreeOfParallelism = 2 },
            song =>
            {
                if (metadataByPath.TryGetValue(song, out var songMetadata))
                {
                    PlaceGenresInStyles(songMetadata);
                    ProcessSongMetadata(songMetadata, styleMappings, stylesToRemove);

                    var metadataHandler = _metadataHandlesFactory.GetHandler(song);
                    metadataHandler.WriteMetadata(song, songMetadata);
                }
                else
                {
                    SongsWithoutMetadata.TryAdd(Path.GetFileNameWithoutExtension(song), 1);
                }
            });

        PrintProcessingResults();
    }

    private void PlaceGenresInStyles(AudioMetadata songSpotDLMetadata)
    {
        songSpotDLMetadata.Styles.Clear();
        songSpotDLMetadata.Styles.AddRange(songSpotDLMetadata.Genres);
        songSpotDLMetadata.Genres.Clear();
    }

    private void ProcessSongMetadata(AudioMetadata songMetadata, Dictionary<string, List<string>> styleMappings, List<string> stylesToRemove)
    {
        var stylesToRemoveSet = new HashSet<string>(stylesToRemove);
        var stylesToKeep = new List<string>();
        var genresToAdd = new List<string>();
        var genreSet = new HashSet<string>();

        foreach (var style in songMetadata.Styles)
        {
            if (styleMappings.TryGetValue(style, out var genres))
            {
                foreach (var genre in genres)
                {
                    if (genreSet.Add(genre))
                    {
                        genresToAdd.Add(genre);
                    }
                }

                if (!stylesToRemoveSet.Contains(style))
                {
                    stylesToKeep.Add(style);
                }
            }
            else
            {
                UnmappedStyles.TryAdd(style, 0);
                stylesToKeep.Add(style);
            }
        }

        songMetadata.Genres.AddRange(genresToAdd);
        songMetadata.Styles.Clear();
        songMetadata.Styles.AddRange(stylesToKeep);
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