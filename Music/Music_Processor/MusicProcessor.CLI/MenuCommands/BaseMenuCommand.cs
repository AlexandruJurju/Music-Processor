using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.Infrastructure;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand(IFileService fileService, IMediator mediator) : ICommand
{
    protected readonly IFileService FileService = fileService;
    protected readonly IMediator Mediator = mediator;

    public abstract ValueTask ExecuteAsync(IConsole console);

    protected bool ValidatePlaylist(string playlistName, IConsole console)
    {
        if (string.IsNullOrEmpty(playlistName))
        {
            console.Error.WriteLine("Please provide a valid playlist name.");
            return false;
        }

        if (!PlaylistExists(playlistName))
        {
            console.Error.WriteLine("Playlist does not exist.");
            DisplayAvailablePlaylists(console);
            return false;
        }

        return true;
    }

    private void DisplayAvailablePlaylists(IConsole console)
    {
        var playlists = FileService.GetAllPlaylistsNames();
        console.Output.WriteLine("Available playlists:");
        foreach (var playlist in playlists) console.Output.WriteLineAsync($"  {playlist}");
    }

    private bool PlaylistExists(string playlistName)
    {
        return FileService.GetAllPlaylistsNames().Contains(playlistName);
    }

    protected string GetPlaylistPath(string playlistName)
    {
        return Path.Combine(FileService.GetPlaylistsPath(), playlistName);
    }
}