using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISongMetadataRepository
{
    Task<SongMetadata?> GetByKeyAsync(string key);
    Task<ICollection<SongMetadata>> GetAllAsync();
    Task<IEnumerable<string>> GetSongTitlesAsync();
    IQueryable<SongMetadata> GetAll();
    Task<SongMetadata?> GetByIdAsync(int id);
    Task UpdateAsync(SongMetadata songMetadata);
    Task DeleteAsync(int id);
    Task AddRangeAsync(List<SongMetadata> songsList);
    Task AddAsync(SongMetadata songMetadata);
    Task UpdateRangeAsync(List<SongMetadata> modifiedSongs);
    Task<Dictionary<string, SongMetadata>> GetAllSongsWithKeyAsync();
}