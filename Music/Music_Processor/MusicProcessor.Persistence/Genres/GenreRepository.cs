using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Persistence.Common;

namespace MusicProcessor.Persistence.Genres;

public class GenreRepository : IGenreRepository
{
    private readonly ApplicationDbContext _context;

    public GenreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetAllAsync()
    {
        return await _context.Genres
            .Include(s => s.GenreCategories)
            .ToListAsync();
    }

    public async Task<Genre> AddAsync(Genre newGenre)
    {
        await _context.Genres.AddAsync(newGenre);
        await _context.SaveChangesAsync();
        return newGenre;
    }

    public Task<Genre?> GetByNameAsync(string styleName)
    {
        return _context.Genres.FirstOrDefaultAsync(s => s.Name == styleName);
    }

    public async Task DeleteAsync(Genre genre)
    {
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string styleName)
    {
        return await _context.Genres.AnyAsync(s => s.Name == styleName);
    }

    public async Task<Genre> UpdateAsync(Genre genre)
    {
        _context.Genres.Update(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task AddRangeAsync(List<Genre> stylesToAdd)
    {
        _context.AddRange(stylesToAdd);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Genre> stylesToUpdate)
    {
        _context.UpdateRange(stylesToUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<Genre> stylesToRemove)
    {
        _context.Genres.RemoveRange(stylesToRemove);
        await _context.SaveChangesAsync();
    }

    public async Task<Genre?> GetByIdAsync(int genreId)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == genreId);
        return genre;
    }
}