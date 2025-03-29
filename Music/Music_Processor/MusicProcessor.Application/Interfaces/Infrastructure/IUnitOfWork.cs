namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}