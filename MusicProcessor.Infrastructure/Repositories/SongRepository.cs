using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Infrastructure.Database;

namespace MusicProcessor.Infrastructure.Repositories;

public class SongRepository(
    ApplicationDbContext dbContext
) : ISongRepository
{
    public async Task<IEnumerable<Song>> GetAllAsync()
    {
        return await dbContext.Songs
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> GetAllWithReferencesAsync()
    {
        return await dbContext.Songs
            .Include(e => e.Artists)
            .Include(e => e.Album)
            .ThenInclude(e => e.MainArtist)
            .Include(e => e.MainArtist)
            .Include(e => e.Styles)
            .ThenInclude(e => e.Genres)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }
    
    public void AddRange(IEnumerable<Song> songs)
    {
        dbContext.Songs.AddRange(songs);
    }

    public async Task BulkInsertAsync(IEnumerable<Song> songs)
    {
        await dbContext.Songs
            .BulkInsertOptimizedAsync(songs, options => options.InsertIfNotExists = true);
    }
}
