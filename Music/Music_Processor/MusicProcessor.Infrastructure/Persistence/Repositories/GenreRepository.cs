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
            .Include(g => g.Styles)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Genre newGenre)
    {
        _context.Genres.Add(newGenre);
        await _context.SaveChangesAsync();
        return newGenre.Id;
    }
}