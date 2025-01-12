using Music_Processor.Model;

namespace Music_Processor.Interfaces;

public interface IPlaylistProcessor
{
    Dictionary<string, SongMetadata> LoadPlaylistMetadata(string playlistPath);
}