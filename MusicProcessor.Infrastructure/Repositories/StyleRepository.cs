using Microsoft.EntityFrameworkCore;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Styles;
using MusicProcessor.Infrastructure.Database;

namespace MusicProcessor.Infrastructure.Repositories;

public class StyleRepository(
    ApplicationDbContext dbContext
) : IStyleRepository
{
    public async Task BulkInsertAsync(IEnumerable<Style> styles)
    {
        await dbContext.Styles.BulkInsertOptimizedAsync(styles, options =>
        {
            options.InsertIfNotExists = true;
            options.ColumnPrimaryKeyExpression = x => new { x.Name };
        });
    }

    public async Task<IEnumerable<Style>> GetAllAsync()
    {
        return await dbContext.Styles.ToListAsync();
    }
}
