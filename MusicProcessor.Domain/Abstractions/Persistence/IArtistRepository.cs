using MusicProcessor.Domain.Artists;

namespace MusicProcessor.Domain.Abstractions.Persistence;

public interface IArtistRepository
{
    Task BulkInsertAsync(IEnumerable<Artist> artists);
    Task<IEnumerable<Artist>> GetAllAsync();
}
