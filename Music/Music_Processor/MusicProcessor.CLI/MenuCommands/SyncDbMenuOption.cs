using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.SyncCommand;

namespace MusicProcessor.CLI.MenuCommands;

public class SyncDbMenuOption : IMenuOption
{
    private readonly IFileService _fileService;
    private readonly IMediator _mediator;

    public SyncDbMenuOption(IFileService fileService, IMediator mediator)
    {
        _fileService = fileService;
        _mediator = mediator;
    }

    public string Name => "Sync Db";

    public async Task ExecuteAsync()
    {
        var baseDirectory = _fileService.GetPlaylistsPath();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);


        foreach (var playlist in availablePlaylists)
        {
            var folderName = Path.GetFileName(playlist);
            Console.WriteLine(folderName);
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return;
        }

        // check if entered playlist name exists in available playlists
        var playlistFolders = availablePlaylists.Select(Path.GetFileName).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }

        var playlistPath = Path.Combine(_fileService.GetPlaylistsPath(), playlistName);
        await _mediator.Send(new SyncDbCommand(playlistPath));
    }
}