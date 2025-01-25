using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataStrategy
{
    Song ReadMetadata(string songPath);
    void WriteMetadata(Song song);
    bool CanHandle(string filePath);
}