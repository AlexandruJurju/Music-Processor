using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Domain.Entities.Albums;

public class Album : BaseEntity
{
    // For EF
    public Album()
    {
    }

    public Album(string name, Artist artist)
    {
        Name = name;
        Artist = artist;
    }

    public string Name { get; set; }
    public AlbumType Type { get; set; }
    public Artist Artist { get; set; }
    public int ArtistId { get; set; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}