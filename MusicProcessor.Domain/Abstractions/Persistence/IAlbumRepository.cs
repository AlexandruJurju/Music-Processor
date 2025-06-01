using MusicProcessor.Domain.Albums;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface IAlbumRepository
{
    Task BulkInsertAsync(IEnumerable<Album> albums);
    Task<IEnumerable<Album>> GetAllAsync();
    void AddRange(IEnumerable<Album> albums);
}
