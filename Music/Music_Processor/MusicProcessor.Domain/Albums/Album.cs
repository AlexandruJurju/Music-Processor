using MusicProcessor.Domain.Abstractions;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Domain.Albums;

public sealed class Album : BaseEntity
{
    public Album(string name, Artist artist)
    {
        Name = name;
        Artist = artist;
    }

    public string Name { get; set; }
    public AlbumType Type { get; set; }
    public Artist Artist { get; set; }
    public int ArtistId { get; set; }
    public ICollection<SongMetadata> Songs { get; set; } = new List<SongMetadata>();

    // For EF
    private Album()
    {
    }
}
