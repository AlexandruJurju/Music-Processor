using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IConfigRepository
{
    Task WriteStyleMappingAsync();
    Task<List<Style>> ReadStyleMappingAsync();
}