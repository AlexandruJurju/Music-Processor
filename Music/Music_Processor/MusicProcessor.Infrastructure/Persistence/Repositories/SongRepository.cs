using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongRepository(ApplicationDbContext context) : ISongRepository
{
    public async Task<ICollection<Song>> GetAllAsync()
    {
        return await GetAll().ToListAsync();
    }

    public async Task<IEnumerable<string>> GetSongTitlesAsync()
    {
        return await context.Songs.Select(s => s.Title).ToListAsync();
    }

    public IQueryable<Song> GetAll()
    {
        return context.Songs
            .Include(s => s.Styles)
            .Include(s => s.Artists);
    }

    public async Task<Song?> GetByIdAsync(int id)
    {
        return await GetAll().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(Song song)
    {
        context.Songs.Update(song);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var song = await context.Songs.FindAsync(id);
        if (song != null)
        {
            context.Songs.Remove(song);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddRangeAsync(List<Song> songsList)
    {
        context.Songs.AddRange(songsList);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Song> modifiedSongs)
    {
        context.Songs.UpdateRange(modifiedSongs);
        await context.SaveChangesAsync();
    }
}