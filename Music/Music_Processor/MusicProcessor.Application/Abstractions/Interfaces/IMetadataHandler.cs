using MusicProcessor.Domain.Model;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataHandler
{
    AudioMetadata ExtractMetadata(string songPath);
    void WriteMetadata(string songPath, AudioMetadata audioMetadata);
}