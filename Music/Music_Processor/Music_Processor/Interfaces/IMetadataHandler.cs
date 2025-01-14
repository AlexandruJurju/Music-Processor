using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataHandler
{
    AudioMetadata ExtractMetadata(string filePath);
    void WriteMetadata(string filePath, AudioMetadata audioMetadata);
    Task WriteMetadataAsync(string filePath, AudioMetadata audioMetadata);
}