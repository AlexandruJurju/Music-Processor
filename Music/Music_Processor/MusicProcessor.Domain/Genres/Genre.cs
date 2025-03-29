using MusicProcessor.Domain.Abstractions;
using MusicProcessor.Domain.GenreCategories;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Domain.Genres;

public sealed class Genre : BaseEntity
{
    public Genre(string name)
    {
        Name = name;
    }

    public Genre(string name, bool removeFromSongs)
    {
        Name = name;
        RemoveFromSongs = removeFromSongs;
    }


    public string Name { get; set; } = string.Empty;
    public ICollection<SongMetadata> Songs { get; set; } = new List<SongMetadata>();
    public ICollection<GenreCategory> GenreCategories { get; set; } = new List<GenreCategory>();
    public bool RemoveFromSongs { get; set; }
    
    private Genre()
    {
    }
}