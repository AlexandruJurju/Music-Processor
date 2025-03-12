using MusicProcessor.Domain.Entities.Albums;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IAlbumRepository
{
    Task<List<Album>> GetAllAsync();
    Task<Album?> GetByNameAsync(string albumName);
    Task AddAsync(Album album);
    Task<Album?> GetByIdAsync(int albumId);
}