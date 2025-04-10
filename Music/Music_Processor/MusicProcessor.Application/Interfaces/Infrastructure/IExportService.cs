using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IExportService
{
    Task ExportMetadata(List<SongMetadata> metadata);
}
