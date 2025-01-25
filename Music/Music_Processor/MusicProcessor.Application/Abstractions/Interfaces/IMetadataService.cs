using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataService
{
    void WriteMetadata(Song song);
    Song ReadMetadata(string filePath);
}