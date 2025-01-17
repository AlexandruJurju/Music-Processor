using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public sealed class Style : BaseEntity
{
    public Style()
    {
    }

    public Style(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Tracks { get; set; } = new List<Song>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public bool RemoveFromSongs { get; set; }
}