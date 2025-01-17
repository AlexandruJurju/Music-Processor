namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IPlaylistProcessor
{
    Task WriteSongsToDbAsync(string playlistPath);
}