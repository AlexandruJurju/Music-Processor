using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface ISpotDLMetadataReader
{
    Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string path);
}
