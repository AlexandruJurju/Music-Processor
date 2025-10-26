using Microsoft.Extensions.Configuration;
using MusicProcessor.Infrastructure.Contracts;

namespace MusicProcessor.Infrastructure;

internal class FileService(IConfiguration configuration) : IFileService
{
    private readonly string[] _audioFileFormats = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".alac", ".aiff", ".opus"];

    public IEnumerable<string> GetAllSongFilePaths()
    {
        return GetAllAudioFilesInFolder(configuration["Paths:MusicFolderPath"]!);
    }

    private IEnumerable<string> GetAllAudioFilesInFolder(string path)
    {
        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => _audioFileFormats.Contains(Path.GetExtension(file)));
    }
}