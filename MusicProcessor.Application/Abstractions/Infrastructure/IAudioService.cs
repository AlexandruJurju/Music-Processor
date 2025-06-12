using MusicProcessor.Domain;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IAudioService
{
    void UpdateSongMetadata(Song song, string songPath);
    Song ReadMetadata(string filePath);
}
