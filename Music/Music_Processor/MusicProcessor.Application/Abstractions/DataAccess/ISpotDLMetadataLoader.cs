using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface ISpotDLMetadataLoader
{
    Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath);
    string CleanKeyName(string name);
}