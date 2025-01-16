using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Model;

public class Style : BaseEntity
{
    public Style()
    {
    }

    public Style(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<AudioMetadata> Tracks { get; set; } = new List<AudioMetadata>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}