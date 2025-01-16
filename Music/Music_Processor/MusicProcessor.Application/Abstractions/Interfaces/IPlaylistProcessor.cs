namespace MusicProcessor.Application.Abstractions.Interfaces;

public interface IPlaylistProcessor
{
    Task FixPlaylistGenresUsingSpotdlMetadataAsync(string playlistPath);
    Task FixPlaylistGenresUsingCustomMetadataAsync(string playlistPath);
}