using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface ISpotDLMetadataReader
{
    Task<IEnumerable<Song>> LoadSpotDLMetadataAsync(string playlistPath);
}