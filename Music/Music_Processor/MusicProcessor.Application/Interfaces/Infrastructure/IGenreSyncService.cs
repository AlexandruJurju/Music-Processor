using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IGenreSyncService
{
    Task WriteStyleMappingAsync();
    Task<IEnumerable<Genre>> ReadStyleMappingAsync();
}