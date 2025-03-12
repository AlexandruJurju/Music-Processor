using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;

namespace MusicProcessor.Domain.Entities.Songs;

public sealed class Song : BaseEntity
{
    private Song()
    {
    }

    public Song(
        string name,
        string isrc,
        ICollection<Artist> artists,
        Artist mainArtist,
        ICollection<Genre> genres,
        int discNumber,
        int discCount,
        Album? album,
        int duration,
        int year,
        DateOnly? date,
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
        Year = year;
        Date = date;
        TrackNumber = trackNumber;
        TracksCount = tracksCount;
        SpotifyInfo = spotifyInfo;
    }

    public string Name { get; set; }
    public string ISRC { get; set; }
    public int? Year { get; set; }
    public DateOnly? Date { get; set; }
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public int DiscNumber { get; set; }
    public int DiscCount { get; set; }
    public int Duration { get; set; }
    public int? AlbumId { get; set; }
    public Album? Album { get; set; }
    public Artist MainArtist { get; set; }
    public int MainArtistId { get; set; }
    public ICollection<Artist> Artists { get; init; } = new List<Artist>();
    public ICollection<Genre> Genres { get; init; } = new List<Genre>();
    public SpotifyInfo? SpotifyInfo { get; init; }
    public string FileType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    private string ValidateFileType(string fileType)
    {
        var allowedFileTypes = new[] { FileTypes.MP3, FileTypes.FLAC };
        if (!allowedFileTypes.Contains(fileType)) throw new ArgumentException($"Invalid file type. Allowed values are: {string.Join(", ", allowedFileTypes)}");

        return fileType;
    }
}