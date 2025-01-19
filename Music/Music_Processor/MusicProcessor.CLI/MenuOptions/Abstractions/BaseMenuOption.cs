using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.CLI.MenuOptions.Abstractions;

public abstract class BaseMenuOption : IMenuOption
{
    protected readonly IFileService _fileService;
    protected readonly IMediator _mediator;

    protected BaseMenuOption(IFileService fileService, IMediator mediator)
    {
        _fileService = fileService;
        _mediator = mediator;
    }

    public abstract string Name { get; }
    public abstract Task ExecuteAsync();

    protected string? GetValidatedPlaylistName(bool validateExistence = true)
    {
        if (validateExistence)
        {
            DisplayAvailablePlaylists();
        }

        Console.Write("Enter playlist name: ");
        var playlistName = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return null;
        }

        if (validateExistence && !PlaylistExists(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return null;
        }

        return playlistName;
    }

    protected void DisplayAvailablePlaylists()
    {
        var baseDirectory = _fileService.GetPlaylistsPath();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);

        foreach (var playlist in availablePlaylists)
        {
            Console.WriteLine(Path.GetFileName(playlist));
        }
    }

    protected bool PlaylistExists(string playlistName)
    {
        var baseDirectory = _fileService.GetPlaylistsPath();
        string[] availablePlaylists = _fileService.GetAllFoldersInPath(baseDirectory);
        var playlistFolders = availablePlaylists.Select(Path.GetFileName).ToList();
        return playlistFolders.Contains(playlistName);
    }

    protected string GetPlaylistPath(string playlistName) => Path.Combine(_fileService.GetPlaylistsPath(), playlistName);
}