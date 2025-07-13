namespace MusicProcessor.Domain;

public class Song
{
    public string Id { get; set; }
    public string Title { get; private set; }
    public string MainArtist { get; private set; }
    public string AlbumName { get; private set; }
    public List<string> Artists { get; private set; }
    public List<string> Styles { get; private set; }
    public int DiscNumber { get; private set; }
    public int DiscCount { get; private set; }
    public int Duration { get; private set; }
    public uint Year { get; private set; }
    public int TrackNumber { get; private set; }
    public int TracksCount { get; private set; }
    public string? Isrc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Song Create(
        string title,
        string mainArtistName,
        List<string>? artists,
        List<string>? styles,
        string albumName,
        int discNumber,
        int discCount,
        int duration,
        uint year,
        int trackNumber,
        int tracksCount,
        string? isrc
    )
    {
        return new Song
        {
            Title = title,
            MainArtist = mainArtistName,
            Artists = artists ?? new List<string>(),
            Styles = styles ?? new List<string>(),
            AlbumName = albumName,
            DiscNumber = discNumber,
            DiscCount = discCount,
            Duration = duration,
            Year = year,
            TrackNumber = trackNumber,
            TracksCount = tracksCount,
            Isrc = isrc,
            CreatedAt = DateTime.UtcNow
        };
    }

    public string GetSongKey() =>
        $"{MainArtist.ToUpperInvariant().Trim()} - {AlbumName.ToUpperInvariant().Trim()} - {Title.ToUpperInvariant().Trim()}";
}