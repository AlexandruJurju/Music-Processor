using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataSerializationStrategy
{
    Task SaveMetadataAsync(List<AudioMetadata> metadata, string playlistName);
    Task<List<AudioMetadata>> LoadMetadataAsync(string playlistName);
    bool CanHandle(string filePath);
}