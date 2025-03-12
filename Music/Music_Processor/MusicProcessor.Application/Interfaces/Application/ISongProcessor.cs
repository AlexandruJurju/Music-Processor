using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Application;

public interface ISongProcessor
{
    Task AddRawSongsToDbAsync(IEnumerable<Song> songs);
}