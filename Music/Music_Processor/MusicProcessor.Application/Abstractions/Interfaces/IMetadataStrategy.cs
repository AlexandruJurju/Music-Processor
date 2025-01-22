using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataStrategy
{
    Song ExtractMetadata(string songPath);
    void UpdateMetadata(Song song);
    bool CanHandle(string filePath);
}