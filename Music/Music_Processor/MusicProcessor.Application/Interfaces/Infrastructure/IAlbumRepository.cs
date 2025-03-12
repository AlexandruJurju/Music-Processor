using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Albums;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IAlbumRepository
{
    Task<List<Album>> GetAllAlbumsAsync();
    Task<Album?> GetAlbumByNameAsync(string albumName);
    Task AddAlbumAsync(Album album);
}