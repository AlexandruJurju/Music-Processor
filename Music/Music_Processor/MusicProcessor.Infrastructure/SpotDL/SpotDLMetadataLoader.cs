using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Model;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLMetadataLoader : ISpotDLMetadataLoader
{
    private readonly IFileService _fileService;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<SpotDLMetadataLoader> _logger;

    public SpotDLMetadataLoader(ILogger<SpotDLMetadataLoader> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<Dictionary<string, AudioMetadata>> LoadSpotDLMetadataAsync(string playlistPath)
    {
        var metadataLookup = new Dictionary<string, AudioMetadata>();

        try
        {
            var spotdlFile = _fileService.GetSpotDLFileInFolder(playlistPath);
            if (spotdlFile is null)
            {
                _logger.LogWarning("No spotdl file found in directory");
                return metadataLookup;
            }

            var jsonContent = await File.ReadAllTextAsync(spotdlFile, Encoding.UTF8);
            var playlistData = JsonSerializer.Deserialize<SpotDLPlaylist>(jsonContent, _jsonOptions);

            if (playlistData?.Songs == null || !playlistData.Songs.Any())
            {
                _logger.LogWarning("No songs found in spotdl file");
                return metadataLookup;
            }

            foreach (var song in playlistData.Songs)
            {
                var metadata = CreateMetadataFromSpotDLSong(song);
                AddMetadataLookups(metadataLookup, metadata, song);
            }

            _logger.LogInformation("Loaded metadata for {Count} songs from spotdl file", metadataLookup.Count);
            return metadataLookup;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void AddMetadataLookups(Dictionary<string, AudioMetadata> metadataLookup, AudioMetadata metadata, SpotDLSongMetadata song)
    {
        var key = CreateLookupKey(song);
        metadataLookup[key] = metadata;
    }

    private AudioMetadata CreateMetadataFromSpotDLSong(SpotDLSongMetadata song)
    {
        return new AudioMetadata
        {
            Title = song.Name,
            Artists = song.Artists.Select(artistName => new Artist(artistName)).ToList(),
            Album = song.AlbumName,
            Genres = song.Genres.Select(genreName => new Genre(CapitalizeGenre(genreName))).ToList(),
            Year = int.TryParse(song.Year, out var year) ? year : null,
            TrackNumber = song.TrackNumber,
            Duration = TimeSpan.FromSeconds(song.Duration)
        };
    }

    public string CreateLookupKey(SpotDLSongMetadata song)
    {
        var cleanArtists = string.Join(", ", song.Artists.Select(CleanKeyName));
        var cleanTitle = CleanKeyName(song.Name);
        return $"{cleanArtists} - {cleanTitle}";
    }

    public string CleanKeyName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        return name.ToLower()
            .Trim()
            .Replace("  ", " ");
    }

    private string CapitalizeGenre(string genre)
    {
        if (string.IsNullOrEmpty(genre))
            return string.Empty;

        return char.ToUpper(genre[0]) + genre[1..].ToLower();
    }
}