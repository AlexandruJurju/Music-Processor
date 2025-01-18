namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IFileService
{
    string GetPlaylistsPath();
    string GetDataAccessFolderPath();
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
    string? GetSpotDLFile(string playlistPath);
}