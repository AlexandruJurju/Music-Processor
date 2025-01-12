using Microsoft.Extensions.Logging;
using Music_Processor.Interfaces;

namespace Music_Processor.CLI.Commands;

public class FixGenresCommand : IMenuCommand
{
    private readonly ILogger<FixGenresCommand> _logger;
    private readonly IFileService _fileService;

    public FixGenresCommand(ILogger<FixGenresCommand> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    public string Name => "Fix Genres";
    public int MenuNumber => MenuChoices.FixGenres;

    public async Task ExecuteAsync()
    {
        string baseDirectory = _fileService.GetPlaylistsDirectory();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);

        foreach (var playlist in availablePlaylists)
        {
            string folderName = Path.GetFileName(playlist);
            Console.WriteLine(folderName);
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();
        if (String.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return;
        }

        // check if entered playlist name exists in available playlists
        var playlistFolders = availablePlaylists.Select(path => Path.GetFileName(path)).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }
        
        
    }
}