using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface IStyleMappingRepository
{
    void Add(GenreMappings genreMappings);
    IEnumerable<GenreMappings> GetAll();
}
