namespace Music_Processor.Interfaces;

public interface IFileService
{
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
    string? GetSpotDLFileInFolder(string path);
}