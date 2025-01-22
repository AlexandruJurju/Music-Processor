using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IStyleRepository
{
    Task<List<Style>> GetAllAsync();
    Task<int> AddAsync(Style newStyle);
    Task<Style?> GetByNameAsync(string styleName);
    Task DeleteAsync(Style style);
    Task<bool> ExistsAsync(string styleName);
    Task<Style> UpdateAsync(Style style);
}