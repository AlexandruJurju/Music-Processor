using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public sealed class Artist : BaseEntity
{
    public Artist()
    {
    }

    public Artist(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Tracks { get; set; } = new List<Song>();
}