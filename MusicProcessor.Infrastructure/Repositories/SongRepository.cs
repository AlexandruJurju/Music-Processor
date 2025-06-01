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
