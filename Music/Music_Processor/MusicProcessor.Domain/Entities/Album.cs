using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public class Album(string name) : BaseEntity
{
    public string Name { get; set; } = name;

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}