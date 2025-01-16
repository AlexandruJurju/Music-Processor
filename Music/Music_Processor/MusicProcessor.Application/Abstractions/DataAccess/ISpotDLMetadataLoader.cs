using MusicProcessor.Domain.Model;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface ISpotDLMetadataLoader
{
    Task<Dictionary<string, AudioMetadata>> LoadSpotDLMetadataAsync(string playlistPath);
    string CleanKeyName(string name);
}