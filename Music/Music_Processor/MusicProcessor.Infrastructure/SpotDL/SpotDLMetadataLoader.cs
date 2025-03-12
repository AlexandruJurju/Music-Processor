using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.Songs;
using MusicProcessor.Domain.Models.SpotDL.Parse;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLMetadataLoader : ISpotDLMetadataLoader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly ILogger<SpotDLMetadataLoader> _logger;
    private readonly IFileService _fileService;

    public SpotDLMetadataLoader(IFileService fileService, ILogger<SpotDLMetadataLoader> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath)
    {
        var spotdlFile = _fileService.GetSpotDLFile(playlistPath);
        if (spotdlFile is null)
        {
            _logger.LogWarning("No spotdl file found in directory");
            return new Dictionary<string, Song>();
        }

        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotdlFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            _logger.LogWarning("No songs found in spotdl file");
            return new Dictionary<string, Song>();
        }

        var metadataLookup = new Dictionary<string, Song>();
        foreach (var song in playlistData.Songs.Select(CreateSongMetadata))
        {
            var key = CreateLookupKey(song.MainArtist, song.Title);
            if (!metadataLookup.TryAdd(key, song)) _logger.LogWarning("Duplicate song found and skipped: {Key}", key);
        }

        _logger.LogInformation("Loaded metadata for {Count} songs from spotdl file", metadataLookup.Count);
        return metadataLookup;
    }

    public string CreateLookupKey(Artist artist, string title)
    {
        // Clean the title
        string cleanedTitle = CleanString(title);

        // Create the final lookup key
        var key = $"{CleanString(artist.Name)} - {cleanedTitle}";

        return key.Replace(";", ", ");
    }


    private Song CreateSongMetadata(SpotDLSongMetadata song)
    {
        return new Song
        {
            Title = song.Name,
            MainArtist = new Artist(song.Artist),
            Artists = song.Artists.Select(name => new Artist(name)).ToList(),
            Album = new Album(song.AlbumName),
            Genres = song.Genres.Select(genre => new Genre(CapitalizeFirstLetter(genre))).ToList(),
            Year = int.TryParse(song.Year, out var year) ? year : null,
            TrackNumber = song.TrackNumber,
            Duration = TimeSpan.FromSeconds(song.Duration)
        };
    }

    private static string CleanString(string input)
    {
        return string.IsNullOrEmpty(input) ? string.Empty : input.ToLower().Trim();
    }

    private static string CapitalizeFirstLetter(string input)
    {
        return string.IsNullOrEmpty(input) ? string.Empty : char.ToUpper(input[0]) + input[1..].ToLower();
    }
}