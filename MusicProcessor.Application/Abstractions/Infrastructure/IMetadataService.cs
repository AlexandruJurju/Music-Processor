using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IMetadataService
{
    Task<List<SpotDLSongMetadata>> LoadSpotDlMetadataAsync();
    Task ExportMetadataAsync(IEnumerable<Song> songs, string path);
}
