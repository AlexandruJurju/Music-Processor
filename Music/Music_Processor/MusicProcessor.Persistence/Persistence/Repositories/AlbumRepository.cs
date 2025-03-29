using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Albums;

namespace MusicProcessor.Persistence.Persistence.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly ApplicationDbContext _db;

    public AlbumRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Album>> GetAllAsync()
    {
        return await _db.Albums.ToListAsync();
    }

    public async Task<Album?> GetByNameAsync(string albumName)
    {
        return await _db.Albums.FirstOrDefaultAsync(a => a.Name == albumName);
    }

    public async Task<Album> AddAsync(Album album)
    {
        await _db.Albums.AddAsync(album);
        await _db.SaveChangesAsync();
        return album;
    }

    public async Task<Album?> GetByIdAsync(int albumId)
    {
        return await _db.Albums.FirstOrDefaultAsync(a => a.Id == albumId);
    }

    public async Task AddRangeAsync(List<Album> newAlbums)
    {
        await _db.AddRangeAsync(newAlbums);
        await _db.SaveChangesAsync();
    }
}