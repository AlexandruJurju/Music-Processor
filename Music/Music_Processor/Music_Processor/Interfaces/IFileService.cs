namespace Music_Processor.Interfaces;

public interface IFileService
{
    string GetBaseDirectory();
    string[] GetAllFoldersInPath(string path);
    string[] GetAllAudioFilesInFolder(string path);
}