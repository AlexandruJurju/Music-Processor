using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadataHandler
{
    SongMetadata ReadMetadata(string songPath);
    void WriteMetadata(SongMetadata songMetadata);
    bool CanHandle(string filePath);
}