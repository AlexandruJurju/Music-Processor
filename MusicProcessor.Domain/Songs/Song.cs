namespace MusicProcessor.Domain.Songs;

public class Song
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<string> Artists { get; set; }
    public string Artist { get; set; }
    public List<string> Genres { get; set; }
    public List<string> Styles { get; set; }
    public int DiscNumber { get; set; }
    public int DiscCount { get; set; }
    public string AlbumName { get; set; }
    public string AlbumArtist { get; set; }
    public int Duration { get; set; }
    public uint Year { get; set; }
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public string SpotifySongId { get; set; }
    public string SpotifyUrl { get; set; }
    public string Isrc { get; set; }
    public string SpotifyCoverUrl { get; set; }
    public string SpotifyAlbumId { get; set; }
    public string SpotifyArtistId { get; set; }
    public string AlbumType { get; set; }

    public string Key => $"{Artist.ToUpperInvariant().Trim()} - {Title.ToUpperInvariant().Trim()}";
}
