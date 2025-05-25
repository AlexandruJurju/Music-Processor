using MusicProcessor.Domain.Abstractions;

namespace MusicProcessor.Domain.Songs;

public class Style : Entity
{
    public string Name { get; set; }
    public List<Genre> Genres { get; set; }
}
