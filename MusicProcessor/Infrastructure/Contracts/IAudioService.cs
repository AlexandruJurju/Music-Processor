using MusicProcessor.Domain;

namespace MusicProcessor.Infrastructure.Contracts;

public interface IAudioService
{
    Task<Song> ReadMetadataAsync(string songFilePath);
}