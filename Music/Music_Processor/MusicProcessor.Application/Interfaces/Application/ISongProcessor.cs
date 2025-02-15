using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Interfaces.Application;

public interface ISongProcessor
{
    Task AddRawSongsToDbAsync(IEnumerable<Song> songs);
}