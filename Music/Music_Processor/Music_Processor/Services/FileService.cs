using Music_Processor.Interfaces;

namespace Music_Processor.Services;

public class FileService : IFileService
{
    public string GetPlaylistsDirectory()
    {
        return Path.Combine(Environment.CurrentDirectory, Constants.Constants.PlaylistFolder);
    }

    public string GetBaseDirectory()
    {
        return Path.Combine(Environment.CurrentDirectory);
    }

    public string[] GetAllFoldersInPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("The path cannot be null or empty.", nameof(path));
        }

        return Directory.GetDirectories(path);
    }

    public string[] GetAllAudioFilesInFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("The path cannot be null or empty.", nameof(path));
        }

        string[] audioFileFormats = Constants.Constants.AudioFileFormats;

        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        return files.Where(file => audioFileFormats.Contains(Path.GetExtension(file).ToLower())).ToArray();
    }
}