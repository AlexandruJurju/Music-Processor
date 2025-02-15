namespace MusicProcessor.Domain.Constants;

public static class FileTypes
{
    public const string MP3 = ".mp3";
    public const string FLAC = ".flac";

    public static string? GetFileType(string extension)
    {
        if (extension.EndsWith(MP3))
            return nameof(MP3);
        if (extension.EndsWith(FLAC))
            return nameof(FLAC);
        return null;
    }
}