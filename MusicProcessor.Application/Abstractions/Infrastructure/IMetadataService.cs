using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain;

namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IMetadataService
{
    Task<List<SongMetadata>> ReadSpotDlMetadataAsync();
    Task ExportMetadataAsync(IEnumerable<Song> songs, string path);
    Task<List<SongMetadata>> ReadFromJsonAsync();
}
