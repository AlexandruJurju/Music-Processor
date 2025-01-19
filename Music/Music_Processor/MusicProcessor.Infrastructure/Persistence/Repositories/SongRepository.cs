using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class SongRepository(ApplicationDbContext context) : ISongRepository
{
    public async Task<ICollection<Song>> GetAllAsync() =>
        await GetAll().ToListAsync();

    public async Task<IEnumerable<string>> GetSongTitlesAsync() =>
        await context.Songs.Select(s => s.Title).ToListAsync();

    public IQueryable<Song> GetAll() =>
        context.Songs
            .Include(s => s.Genres)
            .Include(s => s.Styles)
            .Include(s => s.Artists);

    public async Task<Song?> GetByIdAsync(int id) =>
        await GetAll().FirstOrDefaultAsync(s => s.Id == id);

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
}