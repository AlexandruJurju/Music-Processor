using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISongMetadataRepository
{
    Task<SongMetadata?> GetByKeyAsync(string key);
    Task<List<SongMetadata>> GetAllAsync();
    Task<IEnumerable<string>> GetSongTitlesAsync();
    Task<SongMetadata?> GetByIdAsync(int id);
    Task UpdateAsync(SongMetadata songMetadata);
    Task DeleteAsync(int id);
    Task AddRangeAsync(List<SongMetadata> songsList);
    Task AddAsync(SongMetadata songMetadata);
    Task UpdateRangeAsync(List<SongMetadata> modifiedSongs);
    Task<Dictionary<string, SongMetadata>> GetAllSongsWithKeyAsync();
}
