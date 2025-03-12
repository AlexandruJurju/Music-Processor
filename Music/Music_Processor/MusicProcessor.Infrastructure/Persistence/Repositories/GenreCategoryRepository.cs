﻿using Microsoft.EntityFrameworkCore;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.GenreCategories;

namespace MusicProcessor.Infrastructure.Persistence.Repositories;

public class GenreCategoryRepository(ApplicationDbContext context) : IGenreCategoryRepository
{
    public Task<List<GenreCategory>> GetAllAsync()
    {
        return context.GenreCategories
            .ToListAsync();
    }

    public async Task<GenreCategory> AddAsync(GenreCategory newGenreCategory)
    {
        context.GenreCategories.Add(newGenreCategory);
        await context.SaveChangesAsync();
        return newGenreCategory;
    }

    public async Task<GenreCategory?> GetByNameAsync(string genreName)
    {
        return await context.GenreCategories.FirstOrDefaultAsync(g => g.Name == genreName);
    }

    public async Task DeleteAsync(GenreCategory genreCategory)
    {
        context.GenreCategories.Remove(genreCategory);
        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<GenreCategory> genresToAdd)
    {
        context.GenreCategories.AddRange(genresToAdd);
        await context.SaveChangesAsync();
    }
}