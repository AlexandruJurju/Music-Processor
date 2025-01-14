using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IMetadataService
{
    List<AudioMetadata> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false);
    Task SaveMetadataToJsonAsync(List<AudioMetadata> metadata, string outputJsonPath);
    Task<List<AudioMetadata>> LoadMetadataFromJsonAsync(string jsonPath);
    void WriteSongMetadata(string songPath, AudioMetadata audioMetadata);
    AudioMetadata ExtractMetadataFromSong(string songPath);
}