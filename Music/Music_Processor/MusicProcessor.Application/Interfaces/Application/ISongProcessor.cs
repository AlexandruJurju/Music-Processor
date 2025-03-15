using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Application;

public interface ISongProcessor
{
    Task AddMetadataToDbAsync(IEnumerable<SongMetadata> songs);
}