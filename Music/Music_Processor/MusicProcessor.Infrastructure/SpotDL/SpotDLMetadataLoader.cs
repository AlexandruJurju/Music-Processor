using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Models.SpotDL.Parse;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLMetadataLoader(ILogger<SpotDLMetadataLoader> logger, IFileService fileService) : ISpotDLMetadataLoader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath)
    {
        var spotdlFile = fileService.GetSpotDLFile(playlistPath);
        if (spotdlFile is null)
        {
            logger.LogWarning("No spotdl file found in directory");
            return new Dictionary<string, Song>();
        }

        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotdlFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            return new Dictionary<string, Song>();
        }

        var metadataLookup = new Dictionary<string, Song>();
        foreach (var song in playlistData.Songs.Select(CreateSongMetadata))
        {
            var key = CreateLookupKey(song.Artists, song.Title);
            if (!metadataLookup.TryAdd(key, song)) logger.LogWarning("Duplicate song found and skipped: {Key}", key);
        }

        logger.LogInformation("Loaded metadata for {Count} songs from spotdl file", metadataLookup.Count);
        return metadataLookup;
    }

    public string CreateLookupKey(ICollection<Artist> artists, string title)
    {
        return $"{string.Join(", ", artists.Select(a => CleanString(a.Name)))} - {CleanString(title)}";
    }

    private Song CreateSongMetadata(SpotDLSongMetadata song)
    {
        return new Song
        {
            Title = song.Name,
            Artists = song.Artists.Select(name => new Artist(name)).ToList(),
            Album = song.AlbumName,
            Styles = song.Genres.Select(genre => new Style(CapitalizeFirstLetter(genre))).ToList(),
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