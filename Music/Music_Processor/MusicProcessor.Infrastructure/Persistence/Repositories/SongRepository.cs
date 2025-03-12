using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongRepository : ISongRepository
{
    private readonly ApplicationDbContext _context;

    public SongRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Song>> GetAllAsync()
    {
        return await GetAll().ToListAsync();
    }

    public async Task<IEnumerable<string>> GetSongTitlesAsync()
    {
        return await _context.Songs.Select(s => s.Name).ToListAsync();
    }

    public IQueryable<Song> GetAll()
    {
        return _context.Songs
            .Include(s => s.Genres)
            .ThenInclude(s => s.GenreCategories)
            .Include(s => s.Artists)
            .AsSplitQuery();
    }

    public async Task<Song?> GetByIdAsync(int id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(Song song)
    {
        _context.Songs.Update(song);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var song = await _context.Songs.FindAsync(id);
        if (song != null)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRangeAsync(List<Song> songsList)
    {
        await _context.Songs.AddRangeAsync(songsList);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(Song song)
    {
        await _context.Songs.AddAsync(song);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Song> modifiedSongs)
    {
        _context.Songs.UpdateRange(modifiedSongs);
        await _context.SaveChangesAsync();
    }
}