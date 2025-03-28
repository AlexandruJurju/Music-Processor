﻿using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.GenreCategories;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Domain.Entities.Genres;

public sealed class Genre : BaseEntity
{
    public Genre()
    {
    }

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
}