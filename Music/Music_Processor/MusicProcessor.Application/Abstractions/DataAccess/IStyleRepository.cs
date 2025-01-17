using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IStyleRepository
{
    Task<List<Style>> GetAllAsync();
    Task<int> AddAsync(Style newStyle);
}