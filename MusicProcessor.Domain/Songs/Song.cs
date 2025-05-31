using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Domain.Songs;

public class Song
{
    public int Id { get; set; }
    public string Key { get; private set; }
    public string Title { get; private set; }
    public Artist MainArtist { get; set; }
    public List<Artist> Artists { get; set; }
    public List<Style> Styles { get; set; }
    public Album Album { get; set; }
    public int DiscNumber { get; private set; }
    public int DiscCount { get; private set; }
    public int Duration { get; private set; }
    public uint Year { get; private set; }
    public int TrackNumber { get; private set; }
    public int TracksCount { get; private set; }
    public string Isrc { get; private set; }
    public SpotifyMetadata? SpotifyMetadata { get; private set; }


    public static Song Create(
        string title,
        Artist mainArtist,
        List<Artist> artists,
        List<Style> styles,
        Album album,
        int discNumber,
        int discCount,
        int duration,
        uint year,
        int trackNumber,
        int tracksCount,
        string isrc,
        SpotifyMetadata? spotifyMetadata = null
    )
    {
        return new Song
        {
            Key = $"{mainArtist.Name.ToUpperInvariant().Trim()} - {album.Name.ToUpperInvariant().Trim()} - {title.ToUpperInvariant().Trim()}",
            Title = title,
            MainArtist = mainArtist,
            Artists = artists,
            Styles = styles,
            Album = album,
            DiscNumber = discNumber,
            DiscCount = discCount,
            Duration = duration,
            Year = year,
            TrackNumber = trackNumber,
            TracksCount = tracksCount,
            Isrc = isrc,
            SpotifyMetadata = spotifyMetadata
        };
    }

    protected Song() { }
}
