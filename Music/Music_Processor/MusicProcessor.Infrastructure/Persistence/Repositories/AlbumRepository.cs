using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class AlbumRepository(ApplicationDbContext db) : IAlbumRepository
{
    public async Task<List<Album>> GetAllAlbumsAsync()
    {
        return await db.Albums.ToListAsync();
    }

    public async Task<Album?> GetAlbumByNameAsync(string albumName)
    {
        return await db.Albums.FirstOrDefaultAsync(a => a.Name == albumName);
    }

    public async Task AddAlbumAsync(Album album)
    {
        db.Albums.Add(album);
        await db.SaveChangesAsync();
    }
}