using System.Text.Json;
using System.Text.Json.Serialization;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Infrastructure.ExportService;

public class ExportService : IExportService
{
    public async Task ExportMetadata(List<SongMetadata> metadata)
    {
        var exportData = new List<SongMetadataExport>();

        foreach (SongMetadata song in metadata)
        {
            var export = new SongMetadataExport
            {
                Name = song.Name,
                Artists = song.Artists.Select(a => a.Name).ToList(),
                Artist = song.MainArtist.Name,
                Genres = song.Genres.Select(g => g.Name).ToList(),
                DiscNumber = song.DiscNumber,
                DiscCount = song.DiscCount,
                AlbumName = song.Album?.Name,
                AlbumArtist = song.Album?.Artist.Name,
                Duration = song.Duration,
                Year = song.Date?.ToString(),
                Date = song.Date.HasValue ? $"{song.Date.Value}-01-01" : null,
                TrackNumber = song.TrackNumber,
                TracksCount = song.TracksCount,
                Isrc = song.ISRC,
                AlbumType = song.Album?.Type.ToString()?.ToLower()
            };

            if (song.SpotifyInfo != null)
            {
                export.SpotifySongId = song.SpotifyInfo.SpotifySongId;
                export.Publisher = song.SpotifyInfo.SpotifyPublisher;
                export.SpotifyUrl = song.SpotifyInfo.SpotifySongUrl;
                export.SpotifyCoverUrl = song.SpotifyInfo.SpotifyCoverUrl;
                export.SpotifyAlbumId = song.SpotifyInfo.SpotifyAlbumId;
                export.SpotifyArtistId = song.SpotifyInfo.SpotifyArtistId;
            }

            exportData.Add(export);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string jsonString = JsonSerializer.Serialize(exportData, options);
        await File.WriteAllTextAsync("songs-metadata-export.json", jsonString);
    }
}
