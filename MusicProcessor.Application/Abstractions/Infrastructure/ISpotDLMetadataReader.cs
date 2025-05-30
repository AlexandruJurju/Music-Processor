using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface ISpotDLMetadataReader
{
    Task<List<Song>> LoadSpotDLMetadataAsync();
}
