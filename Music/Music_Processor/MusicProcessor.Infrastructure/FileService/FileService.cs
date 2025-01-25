using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.Infrastructure.FileAccess;

public class FileService : IFileService
{
    public string GetPlaylistsPath()
    {
        return DirectoryPaths.PlaylistsDirectory;
    }

    public string[] GetAllPlaylistsNames()
    {
        var directoriesPaths = Directory.GetDirectories(DirectoryPaths.PlaylistsDirectory);
        return directoriesPaths.Select(Path.GetFileName).ToArray()!;
    }

    public string GetDataAccessFolderPath()
    {
        return DirectoryPaths.BaseDirectory;
    }

    public string[] GetAllFoldersInPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("The path cannot be null or empty.", nameof(path));

        return Directory.GetDirectories(path);
    }

    public string[] GetAllAudioFilesInFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("The path cannot be null or empty.", nameof(path));

        string[] audioFileFormats = Constants.AudioFileFormats;

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        return files.Where(file => audioFileFormats.Contains(Path.GetExtension(file).ToLower())).ToArray();
    }

    public string? GetSpotDLFile(string playlistPath)
    {
        var playlistName = Path.GetFileNameWithoutExtension(playlistPath);

        if (string.IsNullOrWhiteSpace(playlistPath)) throw new ArgumentException("The path cannot be null or empty.", nameof(playlistName));

        var spotdlFile = Directory.GetFiles(DirectoryPaths.PlaylistsDirectory, "*.spotdl", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        return spotdlFile;
    }
}