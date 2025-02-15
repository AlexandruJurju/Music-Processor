namespace MusicProcessor.Infrastructure.FileService;

public static class DirectoryPaths
{
    public static string BaseDirectory => Environment.CurrentDirectory;
    public static string PlaylistsDirectory => Path.Combine(BaseDirectory, "Playlists");
}