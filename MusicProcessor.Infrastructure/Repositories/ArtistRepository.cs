using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Infrastructure.Database;

namespace MusicProcessor.Infrastructure.Repositories;

public class ArtistRepository(
    ApplicationDbContext dbContext
) : IArtistRepository
{
    public async Task BulkInsertAsync(IEnumerable<Artist> artists)
    {
        await dbContext.Artists
            .BulkInsertAsync(artists, options =>
            {
                options.InsertIfNotExists = true;
                options.ColumnPrimaryKeyExpression = x => new { x.Name };
            });
    }

    public void AddRange(IEnumerable<Artist> artists)
    {
        dbContext.Artists.AddRange(artists);
    }

    public async Task<IEnumerable<Artist>> GetAllAsync()
    {
        return await dbContext.Artists.ToListAsync();
    }
}
