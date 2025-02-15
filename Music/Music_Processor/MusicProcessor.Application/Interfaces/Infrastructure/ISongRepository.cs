using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISongRepository
{
    Task<ICollection<Song>> GetAllAsync();
    Task<IEnumerable<string>> GetSongTitlesAsync();
    IQueryable<Song> GetAll();
    Task<Song?> GetByIdAsync(int id);
    Task UpdateAsync(Song song);
    Task DeleteAsync(int id);
    Task AddRangeAsync(List<Song> songsList);
    Task UpdateRangeAsync(List<Song> modifiedSongs);
}