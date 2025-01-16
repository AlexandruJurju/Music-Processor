using MusicProcessor.Domain.Model;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataService
{
    List<AudioMetadata> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false);
    Task SaveMetadataToFileAsync(List<AudioMetadata> metadata, string playlistName);
    Task<List<AudioMetadata>> LoadMetadataFromFileAsync(string playlistName);
    void WriteSongMetadata(string songPath, AudioMetadata audioMetadata);
    AudioMetadata ExtractMetadataFromSong(string songPath);
}