using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Domain.Entities.Albums;

public class Album : BaseEntity
{
    public Album(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    public AlbumType AlbumType { get; init; }
    public ICollection<Song> Songs { get; set; } = new List<Song>();
}