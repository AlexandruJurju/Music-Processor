using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataHandler
{
    Song ExtractMetadata(string songPath);
    void WriteMetadata(string songPath, Song song);
}