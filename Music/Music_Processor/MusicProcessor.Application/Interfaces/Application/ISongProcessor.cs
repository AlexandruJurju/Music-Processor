using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Application;

public interface ISongProcessor
{
    Task AddMetadataToDbAsync(IEnumerable<Song> songs);
}