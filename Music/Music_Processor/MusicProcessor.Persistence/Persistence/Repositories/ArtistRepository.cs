using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Artits;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

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

    public async Task<int> AddAsync(Artist newArtist)
    {
        _db.Artists.Add(newArtist);
        await _db.SaveChangesAsync();
        return newArtist.Id;
    }

    public async Task<Artist?> GetByIdAsync(int artistId)
    {
        return await _db.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
    }

    public async Task AddRangeAsync(List<Artist> newArtists)
    {
        await _db.Artists.AddRangeAsync(newArtists);
        await _db.SaveChangesAsync();
    }
}