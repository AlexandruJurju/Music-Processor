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

    public Style(string name, bool removeFromSongs)
    {
        Name = name;
        RemoveFromSongs = removeFromSongs;
    }


    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public bool RemoveFromSongs { get; set; }
}