using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class GenreRepository(ApplicationDbContext context) : IGenreRepository
{
    public Task<List<Genre>> GetAllAsync()
    {
        return context.Genres
            .ToListAsync();
    }

    public async Task<Genre> AddAsync(Genre newGenre)
    {
        context.Genres.Add(newGenre);
        await context.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre?> GetByNameAsync(string genreName)
    {
        return await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
    }

    public async Task DeleteAsync(Genre genre)
    {
        context.Genres.Remove(genre);
        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Genre> genresToAdd)
    {
        context.Genres.AddRange(genresToAdd);
        await context.SaveChangesAsync();
    }
}