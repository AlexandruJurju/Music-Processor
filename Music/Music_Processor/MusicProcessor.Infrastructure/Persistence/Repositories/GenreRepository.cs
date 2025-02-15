using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class GenreRepository(ApplicationDbContext context) : IGenreRepository
{
    public async Task<List<Genre>> GetAllAsync()
    {
        return await context.Genres
            .Include(s => s.GenreCategories)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Genre newGenre)
    {
        context.Genres.Add(newGenre);
        await context.SaveChangesAsync();
        return newGenre.Id;
    }

    public Task<Genre?> GetByNameAsync(string styleName)
    {
        return context.Genres.FirstOrDefaultAsync(s => s.Name == styleName);
    }

    public async Task DeleteAsync(Genre genre)
    {
        context.Genres.Remove(genre);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string styleName)
    {
        return await context.Genres.AnyAsync(s => s.Name == styleName);
    }

    public async Task<Genre> UpdateAsync(Genre genre)
    {
        context.Genres.Update(genre);
        await context.SaveChangesAsync();
        return genre;
    }

    public async Task AddRangeAsync(List<Genre> stylesToAdd)
    {
        context.AddRange(stylesToAdd);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Genre> stylesToUpdate)
    {
        context.UpdateRange(stylesToUpdate);
        await context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<Genre> stylesToRemove)
    {
        context.Genres.RemoveRange(stylesToRemove);
        await context.SaveChangesAsync();
    }
}