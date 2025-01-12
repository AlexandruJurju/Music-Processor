using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataExtractor
{
    AudioMetadata ExtractMetadata(string filePath);
    void WriteMetadata(string filePath, AudioMetadata audioMetadata);
}