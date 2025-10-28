using MusicProcessor.Domain;

namespace MusicProcessor.Infrastructure.Contracts;

public interface ISongRepository
{
    Task<IEnumerable<Song>> GetAllAsync();
    Task<string> AddAsync(Song song);
    Task AddRangeAsync(IEnumerable<Song> songs);
}