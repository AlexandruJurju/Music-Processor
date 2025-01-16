using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Model;

public class Genre : BaseEntity
{
    public Genre()
    {
    }

    public Genre(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<AudioMetadata> Tracks { get; set; } = new List<AudioMetadata>();
    public ICollection<Style> Styles { get; set; } = new List<Style>();
}