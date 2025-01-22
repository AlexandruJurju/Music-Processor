using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly ApplicationDbContext _context;

    public GenreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Genre>> GetAllAsync()
    {
        return _context.Genres
            .ToListAsync();
    }

    public async Task<Genre> AddAsync(Genre newGenre)
    {
        _context.Genres.Add(newGenre);
        await _context.SaveChangesAsync();
        return newGenre;
    }

    public async Task<Genre?> GetByNameAsync(string genreName)
    {
        return await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
    }

    public async Task DeleteAsync(Genre genre)
    {
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }
}