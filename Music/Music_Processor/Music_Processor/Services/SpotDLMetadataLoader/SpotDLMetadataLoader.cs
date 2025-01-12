using System.Text.Json;
using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services.SpotDLMetadataLoader;

public class SpotdlMetadataLoader
{
    private readonly ILogger<SpotdlMetadataLoader> _logger;
    private readonly IFileService _fileService;
    private readonly JsonSerializerOptions _jsonOptions;

    public SpotdlMetadataLoader(ILogger<SpotdlMetadataLoader> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public Dictionary<string, AudioMetadata> LoadSpotDLMetadata(string playlistPath)
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

            var jsonContent = File.ReadAllText(spotdlFile, System.Text.Encoding.UTF8);
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
        string key = CreateLookupKey(song);
        metadataLookup[key] = metadata;
    }

    private AudioMetadata CreateMetadataFromSpotDLSong(SpotDLSongMetadata song)
    {
        return new AudioMetadata
        {
            Title = song.Name,
            Artists = song.Artists.ToList(),
            Album = song.AlbumName,
            Genres = song.Genres?.Select(CapitalizeGenre).ToList() ?? new List<string>(),
            Year = int.TryParse(song.Year, out int year) ? year : null,
            TrackNumber = song.TrackNumber,
            Duration = TimeSpan.FromSeconds(song.Duration),
        };
    }

    private string CreateLookupKey(SpotDLSongMetadata song)
    {
        var cleanArtist = CleanName(song.Artist);
        var cleanTitle = CleanName(song.Name);
        return $"{cleanArtist} - {cleanTitle}";
    }

    private string CleanName(string name)
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