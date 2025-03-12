namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IFileService
{
    string GetPlaylistsPath();
    string[] GetAllPlaylistsNames();
    string GetDataAccessFolderPath();
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
    string? GetSpotDLFile(string playlistPath);
    string[] GetAllAudioFilesInPlaylist(string playlist);
    IEnumerable<string> GetAllAudioFilesInPath(string playlist);
}