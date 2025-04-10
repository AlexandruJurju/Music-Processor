using Microsoft.Extensions.Options;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Infrastructure.FileService;

public class PhysicalFileService : IFileService
{
    private readonly PathsOptions _pathsOptions;

    private readonly string[] AudioFileFormats = { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".alac", ".aiff", ".opus" };

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
        {
            throw new ArgumentException("The playlist cannot be null or empty.", nameof(path));
        }

        string playlistPath = Path.Combine(_pathsOptions.SpotDLPlaylistsPath, path);

        if (!Directory.Exists(playlistPath))
        {
            throw new DirectoryNotFoundException($"The playlist directory '{playlistPath}' does not exist.");
        }

        return GetAudioFiles(playlistPath);
    }

    public IEnumerable<string> GetAllSpotDLPlaylistsNames()
    {
        string[] directoriesPaths = Directory.GetDirectories(_pathsOptions.SpotDLPlaylistsPath);
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
        string spotdlFilePath = Path.Combine(_pathsOptions.SpotDLPlaylistsPath, playlistName, $"{playlistName}.spotdl");

        return File.Exists(spotdlFilePath) ? spotdlFilePath : null;
    }

    private IEnumerable<string> GetAudioFiles(string path)
    {
        try
        {
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => AudioFileFormats.Contains(Path.GetExtension(file).ToLowerInvariant()));
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to retrieve audio files from '{path}'.", ex);
        }
    }
}
