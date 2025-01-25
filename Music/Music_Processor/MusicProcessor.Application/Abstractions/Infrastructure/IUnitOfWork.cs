namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ClearDatabaseAsync();
}