﻿using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Artits;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly ApplicationDbContext _context;

    public ArtistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Artist>> GetAllAsync()
    {
        return await _context.Artists.ToListAsync();
    }

    public async Task<int> AddAsync(Artist newArtist)
    {
        _context.Artists.Add(newArtist);
        await _context.SaveChangesAsync();
        return newArtist.Id;
    }

    public async Task<Artist?> GetByIdAsync(int artistId)
    {
        return await _context.Artists.FirstOrDefaultAsync(a => a.Id == artistId);
    }
}