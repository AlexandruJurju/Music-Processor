using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface ISpotDLMetadataLoader
{
    Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath);
    string CreateLookupKey(ICollection<Artist> artists, string title);
}