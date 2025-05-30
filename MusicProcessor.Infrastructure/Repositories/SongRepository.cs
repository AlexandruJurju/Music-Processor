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

    public async Task BulkInsertAsync(IEnumerable<Song> songs)
    {
        await dbContext.Songs
            .BulkInsertAsync(songs, options => options.InsertIfNotExists = true);
    }
}
