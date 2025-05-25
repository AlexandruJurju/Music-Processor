using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface ISongRepository
{
    IEnumerable<Song> GetAll();
    Guid Add(Song song);
    bool Update(Song song);
    Dictionary<string, Song> GetAllWithKey();
}
