using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IArtistRepository
{
    Task<List<Artist>> GetAllAsync();
    Task<int> AddAsync(Artist newArtist);
}