﻿using MusicProcessor.Domain.Common;

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

    public Genre(string name, bool removeFromSongs)
    {
        Name = name;
        RemoveFromSongs = removeFromSongs;
    }


    public string Name { get; set; } = string.Empty;
    public ICollection<Song> Songs { get; set; } = new List<Song>();
    public ICollection<GenreCategory> GenreCategories { get; set; } = new List<GenreCategory>();
    public bool RemoveFromSongs { get; set; }
}