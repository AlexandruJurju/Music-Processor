using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IStyleSyncService
{
    Task WriteStyleMappingAsync();
    Task<IEnumerable<Style>> ReadStyleMappingAsync();
}