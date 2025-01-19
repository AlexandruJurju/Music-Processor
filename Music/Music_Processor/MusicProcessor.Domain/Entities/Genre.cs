using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public sealed class Genre : BaseEntity
{
    public Genre()
    {
    }

    public Genre(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
    public ICollection<Style> Styles { get; set; } = new List<Style>();
}