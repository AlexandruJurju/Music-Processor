using MusicProcessor.Domain.GenreCategories;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IGenreCategoryRepository
{
    Task<List<GenreCategory>> GetAllAsync();
    Task<GenreCategory> AddAsync(GenreCategory newGenreCategory);
    Task<GenreCategory?> GetByNameAsync(string genreName);
    Task DeleteAsync(GenreCategory genreCategory);
    Task AddRangeAsync(List<GenreCategory> genresToAdd);
}
