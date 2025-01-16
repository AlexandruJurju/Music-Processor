using MusicProcessor.Domain.Model;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IMetadataSerializationStrategy
{
    Task SaveMetadataAsync(List<AudioMetadata> metadata, string playlistName);
    Task<List<AudioMetadata>> LoadMetadataAsync(string playlistName);
    bool CanHandle(string filePath);
}