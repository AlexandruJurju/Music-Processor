using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IPlaylistProcessor
{
    void FixPlaylistGenresUsingSpotdlMetadata(string playlistPath);
}