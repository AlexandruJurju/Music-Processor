using System.Collections.Concurrent;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly IFileService _fileService;
    private readonly IMetadataService _metadataService;
    private readonly ISpotDLMetadataLoader _spotdlMetadataLoader;
    private readonly ConcurrentDictionary<string, byte> FileWritten = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutMetadata = new();
    private readonly ConcurrentDictionary<string, byte> SongsWithoutStyles = new();
    private readonly ConcurrentDictionary<string, byte> UnmappedStyles = new();

    public PlaylistProcessor(
        IFileService fileService,
        ISpotDLMetadataLoader spotdlMetadataLoader,
        IMetadataService metadataService)
    {
        _fileService = fileService;
        _spotdlMetadataLoader = spotdlMetadataLoader;
        _metadataService = metadataService;
    }

    public async Task FixPlaylistGenresUsingSpotdlMetadataAsync(string playlistPath)
    {
        throw new NotImplementedException();
        // // Load all required data upfront in parallel
        // var spotdlMetadataTask = _spotdlMetadataLoader.LoadSpotDLMetadataAsync(playlistPath);
        // var playlistSongsTask = Task.Run(() => _fileService.GetAllAudioFilesInFolder(playlistPath));
        // var styleMappingsTask = Task.Run(() => _configService.LoadStyleMappingFile());
        // var stylesToRemoveTask = Task.Run(() => _configService.LoadStylesToRemove());
        //
        // await Task.WhenAll(spotdlMetadataTask, playlistSongsTask, styleMappingsTask, stylesToRemoveTask);
        //
        // var spotdlMetadata = spotdlMetadataTask.Result;
        // var playlistSongs = playlistSongsTask.Result;
        // var styleMappings = styleMappingsTask.Result;
        // var stylesToRemove = stylesToRemoveTask.Result;
        //
        // foreach (var song in playlistSongs)
        // {
        //     var songName = Path.GetFileNameWithoutExtension(song);
        //     var cleanedSongName = _spotdlMetadataLoader.CleanKeyName(songName);
        //
        //     if (!spotdlMetadata.TryGetValue(cleanedSongName, out var songMetadata))
        //     {
        //         SongsWithoutMetadata.TryAdd(songName, 1);
        //         continue;
        //     }
        //
        //     PlaceGenresInStyles(songMetadata);
        //     TryUpdateMetadataStylesAndGenres(songMetadata, styleMappings, stylesToRemove);
        //
        //     if (!songMetadata.Styles.Any())
        //     {
        //         SongsWithoutStyles.TryAdd(songName, 1);
        //         continue;
        //     }
        //
        //     _metadataService.WriteSongMetadata(song, songMetadata);
        // }
    }

    public async Task FixPlaylistGenresUsingCustomMetadataAsync(string playlistPath)
    {
        throw new NotImplementedException();

        // var playlistName = Path.GetFileNameWithoutExtension(playlistPath);
        //
        // var customMetadata = await _metadataService.LoadMetadataFromFileAsync(playlistName);
        // var metadataByPath = customMetadata.ToDictionary(m => m.FilePath, m => m);
        // var playlistSongs = _fileService.GetAllAudioFilesInFolder(playlistPath);
        // var styleMappings = _configService.LoadStyleMappingFile();
        // var stylesToRemove = _configService.LoadStylesToRemove();
        //
        // foreach (var song in playlistSongs)
        // {
        //     if (!metadataByPath.TryGetValue(song, out var songMetadata))
        //     {
        //         SongsWithoutMetadata.TryAdd(Path.GetFileNameWithoutExtension(song), 1);
        //         continue;
        //     }
        //
        //     PlaceGenresInStyles(songMetadata);
        //     if (TryUpdateMetadataStylesAndGenres(songMetadata, styleMappings, stylesToRemove))
        //     {
        //         FileWritten.TryAdd(Path.GetFileNameWithoutExtension(song), 1);
        //         _metadataService.WriteSongMetadata(song, songMetadata);
        //     }
        // }
    }

    private bool TryUpdateMetadataStylesAndGenres(Song songMetadata, Dictionary<string, List<string>> styleMappings, List<string> stylesToRemove)
    {
        var originalHash = songMetadata.MetadataHash;

        var stylesToRemoveSet = new HashSet<string>(stylesToRemove);
        var stylesToKeep = new List<Style>();
        var genresToAdd = new List<Genre>();
        var genreSet = new HashSet<string>();

        foreach (var style in songMetadata.Styles)
        {
            if (styleMappings.TryGetValue(style.Name, out var mappedGenres))
            {
                foreach (var genreName in mappedGenres)
                {
                    if (genreSet.Add(genreName))
                    {
                        genresToAdd.Add(new Genre { Name = genreName });
                    }
                }

                if (!stylesToRemoveSet.Contains(style.Name))
                {
                    stylesToKeep.Add(style);
                }
            }
            else
            {
                UnmappedStyles.TryAdd(style.Name, 0);
                stylesToKeep.Add(style);
            }
        }

        songMetadata.Genres = genresToAdd;
        songMetadata.Styles = stylesToKeep;

        var newHash = songMetadata.ComputeHash();

        if (originalHash != newHash)
        {
            Console.WriteLine(songMetadata.Title);
        }

        return originalHash != newHash;
    }

    private void PlaceGenresInStyles(Song songMetadata)
    {
        var currentGenres = songMetadata.Genres.ToList();
        var genreNames = currentGenres.Select(g => g.Name);
        var styleNames = songMetadata.Styles.Select(s => s.Name);

        var newGenres = currentGenres.Where(genre => !styleNames.Contains(genre.Name));

        foreach (var genre in currentGenres)
        {
            songMetadata.Styles.Add(new Style { Name = genre.Name });
        }

        foreach (var genre in newGenres)
        {
            songMetadata.Styles.Add(new Style { Name = genre.Name });
        }

        songMetadata.Genres.Clear();
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