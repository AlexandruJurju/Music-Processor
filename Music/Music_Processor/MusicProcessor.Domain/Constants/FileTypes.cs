namespace MusicProcessor.Domain.Constants;

public static class FileTypes
{
    public const string MP3 = ".mp3";
    public const string FLAC = ".flac";

    public static string? GetFileType(string extension)
    {
        if (extension.EndsWith(FileTypes.MP3))
            return nameof(FileTypes.MP3);
        if (extension.EndsWith(FileTypes.FLAC))
            return nameof(FileTypes.FLAC);
        return null;
    }
}