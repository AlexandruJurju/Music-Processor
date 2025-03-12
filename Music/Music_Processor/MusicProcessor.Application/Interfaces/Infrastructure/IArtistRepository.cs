using MusicProcessor.Domain.Entities.Artits;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IArtistRepository
{
    Task<List<Artist>> GetAllAsync();
    Task<int> AddAsync(Artist newArtist);
    Task<Artist?> GetByIdAsync(int artistId);
}