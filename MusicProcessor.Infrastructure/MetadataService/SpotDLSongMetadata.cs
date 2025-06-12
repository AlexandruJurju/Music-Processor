
namespace MusicProcessor.Infrastructure.MetadataService;

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
    public string Year { get; set; }
    public int TrackNumber { get; set; }
    public int TracksCount { get; set; }
    public string ISRC { get; set; } = string.Empty;
    public string Key => $"{Artist.ToUpperInvariant().Trim()} - {Name.ToUpperInvariant().Trim()}";
}
