namespace MusicProcessor.Application.Abstractions.Infrastructure;

public interface IFileService
{
    IEnumerable<string> GetAllSongPaths();
}
