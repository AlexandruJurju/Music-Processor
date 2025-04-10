using MusicProcessor.Domain.Abstractions;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Domain.Artists;

public sealed class Artist : BaseEntity
{
    public Artist(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<SongMetadata> Songs { get; set; } = new List<SongMetadata>();

    private Artist()
    {
    }
}
