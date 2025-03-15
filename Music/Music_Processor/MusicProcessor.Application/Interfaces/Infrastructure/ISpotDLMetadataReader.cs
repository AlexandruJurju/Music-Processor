using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISpotDLMetadataReader
{
    Task<Dictionary<string, SongMetadata>> LoadSpotDLMetadataAsync(string playlistPath);
}