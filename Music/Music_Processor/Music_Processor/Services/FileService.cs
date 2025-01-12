using Music_Processor.Interfaces;

namespace Music_Processor.Services;

public class FileService : IFileService
{
    public string GetBaseDirectory()
    {
        return Path.Combine(Environment.CurrentDirectory, "Playlists");
    }
}