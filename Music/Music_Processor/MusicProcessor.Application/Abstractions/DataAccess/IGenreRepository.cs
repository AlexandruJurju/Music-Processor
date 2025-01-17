using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync();
    Task<int> AddAsync(Genre newGenre);
}