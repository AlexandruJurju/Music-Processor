using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface ISongRepository
{
    Task<bool> AddAsync(Song songMetadata);
    IQueryable<Song> GetAll();
    Task<List<Song>> GetAllAsync();
    Task<IEnumerable<string>> GetSongTitlesAsync();
    Task AddRangeAsync(List<Song> songsToAdd);
}