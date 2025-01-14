using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataHandler
{
    AudioMetadata ExtractMetadata(string songPath);
    void WriteMetadata(string songPath, AudioMetadata audioMetadata);
}