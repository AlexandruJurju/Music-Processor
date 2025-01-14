using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;
using Music_Processor.Services.SpotDLMetadataLoader;

namespace Music_Processor.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly IFileService _fileService;
    private readonly IMetadataService _metadataService;
    private readonly IConfigService _configService;
    private readonly SpotdlMetadataLoader _spotdlMetadataLoader;

    public PlaylistProcessor(
        IFileService fileService,
        SpotdlMetadataLoader spotdlMetadataLoader,
        IConfigService configService,
        IMetadataService metadataService)
    {
        _fileService = fileService;
        _spotdlMetadataLoader = spotdlMetadataLoader;
        _configService = configService;
        _metadataService = metadataService;
    }

    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ConcurrentDictionary<string, byte> SongsWithoutMetadata = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutStyles = new();
    private readonly ConcurrentDictionary<string, byte> UnmappedStyles = new();

    public async Task FixPlaylistGenresUsingSpotdlMetadataAsync(string playlistPath)
    {
        // Load all required data upfront in parallel
        var spotdlMetadataTask = _spotdlMetadataLoader.LoadSpotDLMetadataAsync(playlistPath);
        var playlistSongsTask = Task.Run(() => _fileService.GetAllAudioFilesInFolder(playlistPath));
        var styleMappingsTask = Task.Run(() => _configService.LoadStyleMappingFile());
        var stylesToRemoveTask = Task.Run(() => _configService.LoadStylesToRemove());

        await Task.WhenAll(spotdlMetadataTask, playlistSongsTask, styleMappingsTask, stylesToRemoveTask);

        var spotdlMetadata = spotdlMetadataTask.Result;
        var playlistSongs = playlistSongsTask.Result;
        var styleMappings = styleMappingsTask.Result;
        var stylesToRemove = stylesToRemoveTask.Result;

        // Prepare all metadata updates first (CPU-bound work)
        var writeTasks = new List<Task>();

        foreach (var song in playlistSongs)
        {
            var songName = Path.GetFileNameWithoutExtension(song);
            var cleanedSongName = _spotdlMetadataLoader.CleanKeyName(songName);

            if (!spotdlMetadata.TryGetValue(cleanedSongName, out var songSpotDLMetadata))
            {
                SongsWithoutMetadata.TryAdd(songName, 1);
                continue;
            }

            // Do CPU-bound work synchronously since we're already in a loop
            PlaceGenresInStyles(songSpotDLMetadata);
            ProcessSongMetadata(songSpotDLMetadata, styleMappings, stylesToRemove);

            if (!songSpotDLMetadata.Styles.Any())
            {
                SongsWithoutStyles.TryAdd(songName, 1);
                continue;
            }

            // Queue up the I/O work without awaiting
            writeTasks.Add(Task.Run(() => _metadataService.WriteSongMetadata(song, songSpotDLMetadata)));
        }

        // Execute all I/O operations concurrently
        await Task.WhenAll(writeTasks);
    }

    public async Task FixPlaylistGenresUsingCustomMetadataAsync(string playlistPath)
    {
        var playlistName = Path.GetFileNameWithoutExtension(playlistPath);

        // Load all required data upfront in parallel
        var jsonContentTask = _metadataService.LoadMetadataFromFileAsync(playlistName);
        var playlistSongsTask = Task.Run(() => _fileService.GetAllAudioFilesInFolder(playlistPath));
        var styleMappingsTask = Task.Run(() => _configService.LoadStyleMappingFile());
        var stylesToRemoveTask = Task.Run(() => _configService.LoadStylesToRemove());

        // Execute all loading tasks concurrently
        await Task.WhenAll(jsonContentTask, playlistSongsTask, styleMappingsTask, stylesToRemoveTask);

        var customMetadata = jsonContentTask.Result;
        var metadataByPath = customMetadata.ToDictionary(m => m.FilePath, m => m);
        var playlistSongs = playlistSongsTask.Result;
        var styleMappings = styleMappingsTask.Result;
        var stylesToRemove = stylesToRemoveTask.Result;

        var writeTasks = new List<Task>();

        foreach (var song in playlistSongs)
        {
            if (!metadataByPath.TryGetValue(song, out var songMetadata))
            {
                SongsWithoutMetadata.TryAdd(Path.GetFileNameWithoutExtension(song), 1);
                continue;
            }

            PlaceGenresInStyles(songMetadata);
            ProcessSongMetadata(songMetadata, styleMappings, stylesToRemove);

            writeTasks.Add(Task.Run(() => _metadataService.WriteSongMetadata(song, songMetadata)));
        }

        await Task.WhenAll(writeTasks);
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