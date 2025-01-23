using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class StyleRepository(ApplicationDbContext context) : IStyleRepository
{
    public async Task<List<Style>> GetAllAsync()
    {
        return await context.Styles
            .Include(s => s.Genres)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Style newStyle)
    {
        context.Styles.Add(newStyle);
        await context.SaveChangesAsync();
        return newStyle.Id;
    }

    public Task<Style?> GetByNameAsync(string styleName)
    {
        return context.Styles.FirstOrDefaultAsync(s => s.Name == styleName);
    }

    public async Task DeleteAsync(Style style)
    {
        context.Styles.Remove(style);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string styleName)
    {
        return await context.Styles.AnyAsync(s => s.Name == styleName);
    }

    public async Task<Style> UpdateAsync(Style style)
    {
        context.Styles.Update(style);
        await context.SaveChangesAsync();
        return style;
    }

    public async Task AddRangeAsync(List<Style> stylesToAdd)
    {
        context.AddRange(stylesToAdd);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Style> stylesToUpdate)
    {
        context.UpdateRange(stylesToUpdate);
        await context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<Style> stylesToRemove)
    {
        context.Styles.RemoveRange(stylesToRemove);
        await context.SaveChangesAsync();
    }
}