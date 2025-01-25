namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IFileService
{
    string GetPlaylistsPath();
    string[] GetAllPlaylistsNames();
    string GetDataAccessFolderPath();
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
    string? GetSpotDLFile(string playlistPath);
}