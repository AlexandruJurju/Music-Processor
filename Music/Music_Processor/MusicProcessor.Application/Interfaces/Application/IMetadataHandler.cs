using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Application;

public interface IMetadataHandler
{
    Song ReadMetadata(string songPath);
    void WriteMetadata(Song song);
    bool CanHandle(string filePath);
}