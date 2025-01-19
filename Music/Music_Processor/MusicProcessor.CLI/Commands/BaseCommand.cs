using System.CommandLine;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.CLI.Commands;

public abstract class BaseCommand(IFileService fileService, IMediator mediator)
{
    protected readonly IFileService FileService = fileService;
    protected readonly IMediator Mediator = mediator;

    public abstract Command CreateSubCommand();

    protected bool ValidatePlaylist(string playlistName)
    {
        if (string.IsNullOrEmpty(playlistName))
        {
            Console.WriteLine("Please provide a valid playlist name.");
            return false;
        }

        if (!PlaylistExists(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            DisplayAvailablePlaylists();
            return false;
        }

        return true;
    }

    protected void DisplayAvailablePlaylists()
    {
        var baseDirectory = FileService.GetPlaylistsPath();
        string[] availablePlaylists = FileService.GetAllFoldersInPath(baseDirectory);

        Console.WriteLine("Available playlists:");
        foreach (var playlist in availablePlaylists)
        {
            Console.WriteLine($"  {Path.GetFileName(playlist)}");
        }
    }

    protected bool PlaylistExists(string playlistName)
    {
        var baseDirectory = FileService.GetPlaylistsPath();
        string[] availablePlaylists = FileService.GetAllFoldersInPath(baseDirectory);
        var playlistFolders = availablePlaylists.Select(Path.GetFileName).ToList();
        return playlistFolders.Contains(playlistName);
    }

    protected string GetPlaylistPath(string playlistName) =>
        Path.Combine(FileService.GetPlaylistsPath(), playlistName);
}