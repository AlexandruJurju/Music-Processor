using Microsoft.Extensions.Configuration;
using MusicProcessor.Application.Abstractions.Infrastructure;

namespace MusicProcessor.Infrastructure.FileService;

public class FileService(
    IConfiguration configuration
) : IFileService
{
    private readonly string[] _audioFileFormats = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".alac", ".aiff", ".opus"];

    public IEnumerable<string> GetAllSongPaths()
    {
        return GetAllAudioFilesInFolder(configuration["Paths:MusicFolderPath"]!);
    }

    private IEnumerable<string> GetAllAudioFilesInFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("The playlist cannot be null or empty.", nameof(path));
        }

        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => _audioFileFormats.Contains(Path.GetExtension(file)));
    }
}
