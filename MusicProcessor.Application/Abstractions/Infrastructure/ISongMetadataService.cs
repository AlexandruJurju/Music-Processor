using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface ISongMetadataService
{
    void UpdateSongMetadata(Song song, string songPath);
    Song ReadMetadata(string filePath);
}
