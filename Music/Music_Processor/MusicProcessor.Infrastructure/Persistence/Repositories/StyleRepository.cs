using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class StyleRepository : IStyleRepository
{
    private readonly ApplicationDbContext _context;

    public StyleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Style>> GetAllAsync()
    {
        return await _context.Styles
            .Include(s => s.Genres)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Style newStyle)
    {
        _context.Styles.Add(newStyle);
        await _context.SaveChangesAsync();
        return newStyle.Id;
    }

    public Task<Style?> GetByNameAsync(string styleName)
    {
        return _context.Styles.FirstOrDefaultAsync(s => s.Name == styleName);
    }

    public async Task DeleteAsync(Style style)
    {
        _context.Styles.Remove(style);
        await _context.SaveChangesAsync();
    }
}