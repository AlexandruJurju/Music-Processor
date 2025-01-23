using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IAlbumRepository
{
    Task<List<Album>> GetAllAlbumsAsync();
    Task<Album?> GetAlbumByNameAsync(string albumName);
    Task AddAlbumAsync(Album album);
}