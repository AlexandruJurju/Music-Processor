using MusicProcessor.Domain.Albums;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IAlbumRepository
{
    Task<List<Album>> GetAllAsync();
    Task<Album?> GetByNameAsync(string albumName);
    Task<Album> AddAsync(Album album);
    Task<Album?> GetByIdAsync(int albumId);
    Task AddRangeAsync(List<Album> newAlbums);
}
