using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IStyleRepository
{
    Task<List<Style>> GetAllAsync();
    Task<int> AddAsync(Style newStyle);
    Task<Style?> GetByNameAsync(string styleName);
    Task DeleteAsync(Style style);
    Task<bool> ExistsAsync(string styleName);
    Task<Style> UpdateAsync(Style style);
    Task AddRangeAsync(List<Style> stylesToAdd);
    Task UpdateRangeAsync(List<Style> stylesToUpdate);
    Task RemoveRangeAsync(List<Style> stylesToRemove);
}