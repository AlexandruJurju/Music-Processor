using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface ISpotDLMetadataReader
{
    Task<List<SpotDLSongMetadata>> LoadSpotDLMetadataAsync();
}
