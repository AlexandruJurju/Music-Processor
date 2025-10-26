namespace MusicProcessor.Infrastructure.Contracts;

public interface IFileService
{
    IEnumerable<string> GetAllSongFilePaths();
}