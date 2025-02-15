namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ClearDatabaseAsync();
}