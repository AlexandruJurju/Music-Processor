using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISpotDLMetadataLoader
{
    Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath);
    string CreateLookupKey(Artist artist, string title);
}