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

    public IEnumerable<string> GetAllMainMusicFiles()
    {
        return GetAllAudioFilesInFolder(_pathsOptions.MusicFolderPath);
    }

    public IEnumerable<string> GetAllSpotDlMusicFiles()
    {
        return GetAllAudioFilesInFolder(_pathsOptions.SpotDLPlaylistsPath);
    }

    public IEnumerable<string> GetAllAudioFilesInFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("The playlist cannot be null or empty.", nameof(path));

        string playlistPath = Path.Combine(_pathsOptions.SpotDLPlaylistsPath, path);

        if (!Directory.Exists(playlistPath))
            throw new DirectoryNotFoundException($"The playlist directory '{playlistPath}' does not exist.");

        return GetAudioFiles(playlistPath);
    }

    private IEnumerable<string> GetAudioFiles(string path)
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

    public IEnumerable<string> GetAllSpotDLPlaylistsNames()
    {
        var directoriesPaths = Directory.GetDirectories(_pathsOptions.SpotDLPlaylistsPath);
        return directoriesPaths.Select(Path.GetFileName)!;
    }

    public string GetMainMusicFolderPath()
    {
        return _pathsOptions.MusicFolderPath;
    }

    public string GetSpotDlPlaylistsPath()
    {
        return _pathsOptions.SpotDLPlaylistsPath;
    }

    public string? GetSpotDLFileInPlaylistFolder(string playlistName)
    {
        // Construct the full path to the .spotdl file
        var spotdlFilePath = Path.Combine(_pathsOptions.SpotDLPlaylistsPath, playlistName, $"{playlistName}.spotdl");

        return File.Exists(spotdlFilePath) ? spotdlFilePath : null;
    }
}