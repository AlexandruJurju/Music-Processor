namespace Music_Processor.Interfaces;

public interface IFileService
{
    string GetPlaylistsDirectory();
    string GetBaseDirectory();
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
    string GetMetadataStorageFileForPlaylist(string playlistPath);
}