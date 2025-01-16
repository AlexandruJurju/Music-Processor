using MusicProcessor.Domain.Common;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);
    IQueryable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}