using MusicProcessor.Domain.Abstractions;
using MusicProcessor.Domain.Genres;

namespace MusicProcessor.Domain.GenreCategories;

public sealed class GenreCategory : BaseEntity
{
    public GenreCategory(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();

    private GenreCategory()
    {
    }
}
