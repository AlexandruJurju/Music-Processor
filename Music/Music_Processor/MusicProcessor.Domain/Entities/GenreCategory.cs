using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public sealed class GenreCategory : BaseEntity
{
    public GenreCategory()
    {
    }

    public GenreCategory(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}