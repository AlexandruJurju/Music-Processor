namespace Music_Processor.Interfaces;

public interface IPlaylistProcessor
{
    Task FixPlaylistGenresUsingSpotdlMetadataAsync(string playlistPath);
    void FixPlaylistGenresUsingCustomMetadata(string playlistPath, string metadataPath);
}