using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface ISongProcessor
{
    Task AddRawSongToDbAsync(IEnumerable<Song> songs);
}