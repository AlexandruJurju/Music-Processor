namespace Music_Processor.Interfaces;

public interface IPlaylistProcessor
{
    Task FixPlaylistGenresUsingSpotdlMetadataAsync(string playlistPath);
    Task FixPlaylistGenresUsingCustomMetadataAsync(string playlistPath, string metadataPath);
}