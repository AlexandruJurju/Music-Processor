using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadataService
{
    void WriteMetadata(SongMetadata songMetadata, string songPath);
    SongMetadata ReadMetadata(string songPath);
}
