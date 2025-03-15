using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Domain.Entities.GenreCategories;

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
    public ICollection<SongMetadata> Songs { get; set; } = new List<SongMetadata>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}