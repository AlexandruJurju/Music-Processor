using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand : ICommand
{
    protected readonly IFileService _fileService;
    protected readonly IMediator _mediator;

    protected BaseMenuCommand(IFileService fileService, IMediator mediator)
    {
        _fileService = fileService;
        _mediator = mediator;
    }

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
        var playlists = _fileService.GetAllPlaylistsNames();
        console.Output.WriteLine("Available playlists:");
        foreach (var playlist in playlists) console.Output.WriteLineAsync($"  {playlist}");
    }

    private bool PlaylistExists(string playlistName)
    {
        return _fileService.GetAllPlaylistsNames().Contains(playlistName);
    }

    protected string GetPlaylistPath(string playlistName)
    {
        return Path.Combine(_fileService.GetPlaylistsPath(), playlistName);
    }
}