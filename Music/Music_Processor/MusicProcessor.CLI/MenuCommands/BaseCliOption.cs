using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseCommand : ICommand
{
    protected readonly IFileService FileService;
    protected readonly IMediator Mediator;

    protected BaseCommand(IFileService fileService, IMediator mediator)
    {
        FileService = fileService;
        Mediator = mediator;
    }

    public abstract ValueTask ExecuteAsync(IConsole console);

    protected bool ValidatePlaylist(string playlistName, IConsole console)
    {
        if (string.IsNullOrEmpty(playlistName))
        {
            console.Error.WriteLineAsync("Please provide a valid playlist name.");
            return false;
        }

        if (!PlaylistExists(playlistName))
        {
            console.Error.WriteLineAsync("Playlist does not exist.");
            DisplayAvailablePlaylists(console);
            return false;
        }

        return true;
    }

    protected void DisplayAvailablePlaylists(IConsole console)
    {
        var playlists = FileService.GetAllPlaylistsNames();
        console.Output.WriteLineAsync("Available playlists:");
        foreach (var playlist in playlists) console.Output.WriteLineAsync($"  {playlist}");
    }

    protected bool PlaylistExists(string playlistName)
    {
        return FileService.GetAllPlaylistsNames().Contains(playlistName);
    }

    protected string GetPlaylistPath(string playlistName)
    {
        return Path.Combine(FileService.GetPlaylistsPath(), playlistName);
    }
}