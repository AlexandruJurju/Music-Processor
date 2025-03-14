namespace MusicProcessor.Application.Interfaces.Infrastructure;

public interface IFileService
{
    IEnumerable<string> GetAllMainMusicFiles();
    IEnumerable<string> GetAllSpotDlMusicFiles();
    IEnumerable<string> GetAllAudioFilesInFolder(string path);
    IEnumerable<string> GetAllSpotDLPlaylistsNames();
    string GetMainMusicFolderPath();
    string GetSpotDlPlaylistsPath();
    string? GetSpotDLFileInPlaylistFolder(string playlistName);
}