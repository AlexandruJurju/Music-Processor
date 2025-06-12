using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain;

namespace MusicProcessor.Infrastructure.MetadataService;

public class MetadataService(
    IConfiguration config,
    ILogger<MetadataService> logger
) : IMetadataService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    public async Task<List<SongMetadata>> ReadSpotDlMetadataAsync()
    {
        await using FileStream fileStream = File.OpenRead(config["Paths:SpotdlMetadataFile"]!);
        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(fileStream, _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        return playlistData.Songs.Select(s=> new SongMetadata
        {
            AlbumArtist = s.AlbumArtist,
            AlbumName = s.AlbumName,
            Artists = s.Artists,
            DiscCount = s.DiscCount,
            DiscNumber = s.DiscNumber,
            Duration = s.Duration,
            ISRC = s.ISRC,
            Artist = s.Artist,
            Name = s.Name,
            Styles = s.Genres,
            TrackNumber = s.TrackNumber,
            TracksCount = s.TracksCount,
            Year = uint.Parse(s.Year, CultureInfo.InvariantCulture),
        }).ToList();
    }

    public async Task ExportMetadataAsync(IEnumerable<Song> songs, string path)
    {
        // Convert Songs to SongMetadata for export
        var songMetadataList = songs.Select(song => new SongMetadata
        {
            Name = song.Title,
            Artist = song.MainArtist,
            Artists = song.Artists.ToArray(),
            Styles = song.Styles.ToArray(),
            DiscNumber = song.DiscNumber,
            DiscCount = song.DiscCount,
            AlbumName = song.AlbumName,
            AlbumArtist = song.MainArtist,
            Duration = song.Duration,
            Year = song.Year,
            TrackNumber = song.TrackNumber,
            TracksCount = song.TracksCount,
            ISRC = song.Isrc ?? string.Empty
        }).ToList();

        string json = JsonSerializer.Serialize(songMetadataList, _jsonOptions);
        await File.WriteAllTextAsync(path, json);
    }

    public async Task<List<SongMetadata>> ReadFromJsonAsync()
    {
        await using FileStream fileStream = File.OpenRead(config["Paths:ExportedMetadata"]!);
        List<SongMetadata> songs = await JsonSerializer.DeserializeAsync<List<SongMetadata>>(fileStream, _jsonOptions);

        if (songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in exported metadata file");
            return new List<SongMetadata>();
        }

        return songs;
    }
}
