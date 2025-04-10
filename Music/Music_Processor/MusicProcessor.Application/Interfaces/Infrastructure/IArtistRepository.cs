using MusicProcessor.Domain.Artists;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IArtistRepository
{
    Task<List<Artist>> GetAllAsync();
    Task<Artist> AddAsync(Artist newArtist);
    Task<Artist?> GetByIdAsync(int artistId);
    Task AddRangeAsync(List<Artist> newArtists);
    Task<Artist?> GetByNameAsync(string artistName);
}
