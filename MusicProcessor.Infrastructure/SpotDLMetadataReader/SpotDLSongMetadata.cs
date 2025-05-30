using System.Globalization;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Domain.Styles;

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

    public Song ToSong(Artist mainArtist, List<Artist> artists, Album album, List<Style> styles)
    {
        return Song.Create(
            title: Name,
            mainArtist: mainArtist,
            artists: artists,
            styles: styles,
            album: album,
            discNumber: DiscNumber,
            discCount: DiscCount,
            duration: Duration,
            year: uint.Parse(Year, CultureInfo.InvariantCulture),
            trackNumber: TrackNumber,
            tracksCount: TracksCount,
            isrc: ISRC,
            spotifyMetadata: new SpotifyMetadata
            {
                SpotifySongId = SongId,
                SpotifyUrl = Url,
                SpotifyCoverUrl = CoverUrl,
                SpotifyArtistId = ArtistId,
                SpotifyAlbumId = AlbumId
            }
        );
    }
}
