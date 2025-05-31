using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Infrastructure.Database;

namespace MusicProcessor.Infrastructure.Repositories;

public class AlbumRepository(
    ApplicationDbContext dbContext
) : IAlbumRepository
{
    public async Task BulkInsertAsync(IEnumerable<Album> albums)
    {
        await dbContext.Albums.BulkInsertAsync(albums, options =>
        {
            options.InsertIfNotExists = true;
            options.ColumnPrimaryKeyExpression = x => new { x.Name };
        });
    }

    public async Task<IEnumerable<Album>> GetAllAsync()
    {
        return await dbContext.Albums.ToListAsync();
    }
}
