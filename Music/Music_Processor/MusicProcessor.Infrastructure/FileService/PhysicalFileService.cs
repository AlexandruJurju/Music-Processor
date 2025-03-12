using Microsoft.Extensions.Options;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.Infrastructure.FileService;

public class PhysicalFileService : IFileService
{
    private readonly PathsOptions _pathsOptions;

    public PhysicalFileService(IOptions<PathsOptions> pathsOptions)
    {
        _pathsOptions = pathsOptions.Value;
    }

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

    public string[] GetAllAudioFilesInPlaylist(string playlist)
    {
        if (string.IsNullOrWhiteSpace(playlist))
            throw new ArgumentException("The playlist cannot be null or empty.", nameof(playlist));

        var files = Directory.EnumerateFiles(
            Path.Combine(DirectoryPaths.PlaylistsDirectory, playlist),
            "*.*",
            SearchOption.AllDirectories);

        return files.Where(file => Constants.AudioFileFormats.Contains(Path.GetExtension(file).ToLower())).ToArray();
    }

    public IEnumerable<string> GetAllAudioFilesInPath(string playlist)
    {
        if (string.IsNullOrWhiteSpace(playlist))
            throw new ArgumentException("The playlist cannot be null or empty.", nameof(playlist));

        string playlistPath = Path.Combine(DirectoryPaths.PlaylistsDirectory, playlist);

        if (!Directory.Exists(playlistPath))
            throw new DirectoryNotFoundException($"The playlist directory '{playlistPath}' does not exist.");

        return GetAudioFiles(playlistPath);
    }

    private static IEnumerable<string> GetAudioFiles(string path)
    {
        try
        {
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => Constants.AudioFileFormats.Contains(Path.GetExtension(file).ToLowerInvariant()));
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to retrieve audio files from '{path}'.", ex);
        }
    }


    public string[] GetAllAudioFilesInFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("The path cannot be null or empty.", nameof(path));

        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);

        return files.Where(file => Constants.AudioFileFormats.Contains(Path.GetExtension(file).ToLower())).ToArray();
    }

    public string? GetSpotDLFile(string playlistPath)
    {
        var playlistName = Path.GetFileNameWithoutExtension(playlistPath);

        if (string.IsNullOrWhiteSpace(playlistPath))
            throw new ArgumentException("The path cannot be null or empty.", nameof(playlistName));

        var spotdlFile = Directory.GetFiles(DirectoryPaths.PlaylistsDirectory, "*.spotdl", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        return spotdlFile;
    }
}