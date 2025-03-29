using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;

namespace MusicProcessor.Domain.Entities.SongsMetadata;

public sealed class SongMetadata : BaseEntity
{
    private SongMetadata()
    {
    }

    public SongMetadata(
        string name,
        string isrc,
        ICollection<Artist> artists,
        Artist mainArtist,
        ICollection<Genre> genres,
        int discNumber,
        int discCount,
        Album? album,
        int duration,
        int date,
        int trackNumber,
        int tracksCount,
        SpotifyInfo? spotifyInfo = null
    )
    {
        Name = name;
        ISRC = isrc;
        Artists = artists;
        MainArtist = mainArtist;
        Genres = genres;
        DiscNumber = discNumber;
        DiscCount = discCount;
        Album = album;
        Duration = duration;
        Date = date;
        TrackNumber = trackNumber;
        TracksCount = tracksCount;
        SpotifyInfo = spotifyInfo;
    }

    public string Name { get; set; }
    public string ISRC { get; set; }
    public int? Date { get; set; }
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public int DiscNumber { get; set; }
    public int DiscCount { get; set; }
    public int Duration { get; set; }
    public int? AlbumId { get; set; }
    public Album? Album { get; set; }
    public Artist MainArtist { get; set; }
    public int MainArtistId { get; set; }
    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public SpotifyInfo? SpotifyInfo { get; set; }
    public string Key => $"{MainArtist.Name.ToLower().Trim()} - {Name.ToLower().Trim()}";
}