using MusicProcessor.Domain.Entities.Genres;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<int> AddAsync(Genre newGenre);
    Task<Genre?> GetByNameAsync(string styleName);
    Task DeleteAsync(Genre genre);
    Task<bool> ExistsAsync(string styleName);
    Task<Genre> UpdateAsync(Genre genre);
    Task AddRangeAsync(List<Genre> stylesToAdd);
    Task UpdateRangeAsync(List<Genre> stylesToUpdate);
    Task RemoveRangeAsync(List<Genre> stylesToRemove);
    Task<Genre?> GetByIdAsync(int genreId);
}