using System.Globalization;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.SpotDLMetadataReader;

public sealed class SpotDLSongMetadata
{
    public string Name { get; set; } = string.Empty;
    public string[] Artists { get; set; } = [];
    public string Artist { get; set; } = string.Empty;
    public string[] Genres { get; set; } = [];
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
    public int? ListPosition { get; set; }
    public int? ListLength { get; set; }
    public string ArtistId { get; set; } = string.Empty;
    public string AlbumType { get; set; } = string.Empty;
    public string Key => $"{Artist.ToUpperInvariant().Trim()} - {Name.ToUpperInvariant().Trim()}";

    public Song ToSong()
    {
        return new Song
        {
            Title = Name,
            Artist = Artist,
            Artists = Artists.ToList(),
            DiscCount = DiscCount,
            DiscNumber = DiscNumber,
            AlbumName = AlbumName,
            AlbumType = AlbumType,
            AlbumArtist = AlbumArtist,
            Duration = Duration,
            TrackNumber = TrackNumber,
            TracksCount = TracksCount,
            Year = uint.Parse(Year, CultureInfo.InvariantCulture),
            SpotifySongId = SongId,
            SpotifyUrl = Url,
            SpotifyCoverUrl = CoverUrl,
            SpotifyArtistId = ArtistId,
            SpotifyAlbumId = AlbumId,
            Styles = Genres.ToList(),
            Genres = []
        };
    }
}
