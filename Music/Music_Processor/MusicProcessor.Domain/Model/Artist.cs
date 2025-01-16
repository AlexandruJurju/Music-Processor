using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Model;

public class Artist : BaseEntity
{
    public Artist()
    {
    }

    public Artist(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<AudioMetadata> Tracks { get; set; } = new List<AudioMetadata>();
}