using MusicProcessor.Domain.Abstractions;

namespace MusicProcessor.Domain.Songs;

public class Genre : Entity
{
    public string Name { get; set; }
    public List<Style> Styles { get; set; }
}
