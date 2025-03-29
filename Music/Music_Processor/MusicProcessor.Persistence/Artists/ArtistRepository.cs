using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Persistence.Common;

namespace MusicProcessor.Persistence.Artists;

public class ArtistRepository : IArtistRepository
{
    private readonly ApplicationDbContext _db;

    public ArtistRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Artist>> GetAllAsync()
    {
        return await _db.Artists.ToListAsync();
    }

    public async Task<Artist> AddAsync(Artist newArtist)
    {
        await _db.Artists.AddAsync(newArtist);
        await _db.SaveChangesAsync();
        return newArtist;
    }

    public async Task<Artist?> GetByIdAsync(int artistId)
    {
        return await _db.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
    }

    public async Task<Artist?> GetByNameAsync(string artistName)
    {
        return await _db.Artists.FirstOrDefaultAsync(a => a.Name == artistName);
    }

    public async Task AddRangeAsync(List<Artist> newArtists)
    {
        await _db.Artists.AddRangeAsync(newArtists);
        await _db.SaveChangesAsync();
    }
}