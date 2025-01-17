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
        return await _context.Styles.ToListAsync();
    }

    public async Task<int> AddAsync(Style newStyle)
    {
        _context.Styles.Add(newStyle);
        await _context.SaveChangesAsync();
        return newStyle.Id;
    }
}