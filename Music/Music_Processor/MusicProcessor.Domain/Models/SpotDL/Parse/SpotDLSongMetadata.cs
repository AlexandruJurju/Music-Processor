namespace MusicProcessor.Domain.Models.SpotDL.Parse;

public sealed class SpotDLSongMetadata
{
    public string Name { get; set; } = string.Empty;
    public string[] Artists { get; set; } = Array.Empty<string>();
    public string Artist { get; set; } = string.Empty;
    public string[] Genres { get; set; } = Array.Empty<string>();
    public int DiscNumber { get; set; }
    public int DiscCount { get; set; }
    public string AlbumName { get; set; } = string.Empty;
    public string AlbumArtist { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Year { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public string SongId { get; set; } = string.Empty;
    public bool Explicit { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ISRC { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public string CopyrightText { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
    public string? Lyrics { get; set; }
    public int Popularity { get; set; }
    public string AlbumId { get; set; } = string.Empty;
    public string ListName { get; set; } = string.Empty;
    public string ListUrl { get; set; } = string.Empty;
    public int ListPosition { get; set; }
    public int ListLength { get; set; }
    public string ArtistId { get; set; } = string.Empty;
    public string AlbumType { get; set; } = string.Empty;
}