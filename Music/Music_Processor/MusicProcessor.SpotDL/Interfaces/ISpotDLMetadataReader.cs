using MusicProcessor.SpotDL.Models;

namespace MusicProcessor.SpotDL.Interfaces;

public interface ISpotDLMetadataReader
{
    Task<Dictionary<string, SpotDLSongMetadata>> LoadSpotDLMetadataAsync(string spotDLFile);
}