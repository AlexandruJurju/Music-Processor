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
}
