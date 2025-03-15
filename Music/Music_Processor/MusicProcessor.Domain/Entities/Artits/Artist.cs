using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Domain.Entities.Artits;

public sealed class Artist : BaseEntity
{
    public Artist()
    {
    }

    public Artist(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;
    public ICollection<SongMetadata> Songs { get; set; } = new List<SongMetadata>();
}