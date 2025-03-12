using MusicProcessor.Domain.Entities.Genres;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IGenreSyncService
{
    Task WriteStyleMappingAsync();
    Task<IEnumerable<Genre>> ReadStyleMappingAsync();
}