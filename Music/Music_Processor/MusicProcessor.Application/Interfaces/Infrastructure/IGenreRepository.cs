using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<Genre> AddAsync(Genre newGenre);
    Task<Genre?> GetByNameAsync(string genreName);
    Task DeleteAsync(Genre genre);
    Task AddRangeAsync(List<Genre> genresToAdd);
}