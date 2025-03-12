using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Albums;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class AlbumRepository(ApplicationDbContext db) : IAlbumRepository
{
    public async Task<List<Album>> GetAllAsync()
    {
        return await db.Albums.ToListAsync();
    }

    public async Task<Album?> GetByNameAsync(string albumName)
    {
        return await db.Albums.FirstOrDefaultAsync(a => a.Name == albumName);
    }

    public async Task AddAsync(Album album)
    {
        db.Albums.Add(album);
        await db.SaveChangesAsync();
    }

    public async Task<Album?> GetByIdAsync(int albumId)
    {
        return await db.Albums.FirstOrDefaultAsync(a => a.Id == albumId);
    }
}