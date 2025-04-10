using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadatImportService
{
    Task ImportSongMetadataAsync(IEnumerable<SongMetadata> songs);
}
