namespace MusicProcessor.Infrastructure.ExportService;

public class SongMetadataExport
{
    public string Name { get; set; } = null!;
    public List<string> Artists { get; set; } = null!;
    public string Artist { get; set; } = null!;
    public List<string> Genres { get; set; } = null!;
    public int DiscNumber { get; set; }
    public int DiscCount { get; set; }
    public string? AlbumName { get; set; }
    public string? AlbumArtist { get; set; }
    public int Duration { get; set; }
    public string? Year { get; set; }
    public string? Date { get; set; }
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string? AlbumType { get; set; }
    public string Isrc { get; set; } = null!;
    public string? SpotifyUrl { get; set; }
    public string? SpotifySongId { get; set; }
    public string? SpotifyCoverUrl { get; set; }
    public string? SpotifyAlbumId { get; set; }
    public string? SpotifyArtistId { get; set; }
}
