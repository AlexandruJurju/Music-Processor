using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IMetadataService
{
    List<Song> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false);
    void WriteSongMetadata(string songPath, Song song);
    Song ExtractMetadataFromSong(string songPath);
}