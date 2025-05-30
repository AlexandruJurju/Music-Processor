using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface ISongRepository
{
    Task<IEnumerable<Song>> GetAllAsync();
    Task BulkInsertAsync(IEnumerable<Song> songs);
}
