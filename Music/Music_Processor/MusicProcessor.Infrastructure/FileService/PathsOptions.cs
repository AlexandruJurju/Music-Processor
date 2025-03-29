namespace MusicProcessor.Infrastructure.FileService;

public sealed class PathsOptions
{
    public string MusicFolderPath { get; init; } = string.Empty;
    public string SpotDLPlaylistsPath { get; init; } = string.Empty;
    public string PersistencePath { get; init; } = string.Empty;
    public string SQLLiteConnection { get; init; } = string.Empty;
    public string LogsPath { get; init; } = string.Empty;
}