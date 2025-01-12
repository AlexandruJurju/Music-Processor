using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataService
{
    Task<List<AudioMetadata>> ProcessDirectoryAsync(string directoryPath, bool recursive = false);
    Task SaveMetadataToJsonAsync(List<AudioMetadata> metadata, string outputJsonPath);
    Task<List<AudioMetadata>> LoadMetadataFromJsonAsync(string jsonPath);
}