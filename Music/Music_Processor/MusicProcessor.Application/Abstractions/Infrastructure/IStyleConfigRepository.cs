using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IStyleConfigRepository
{
    Task WriteStyleMappingAsync();
    Task<IEnumerable<Style>> ReadStyleMappingAsync();
}