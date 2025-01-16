using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.CLI.MenuCommands;

public class FixGenresSpotDLCommand : IMenuCommand
{
    private readonly IFileService _fileService;
    private readonly IPlaylistProcessor _playlistProcessor;

    public FixGenresSpotDLCommand(IFileService fileService, IPlaylistProcessor playlistProcessor)
    {
        _fileService = fileService;
        _playlistProcessor = playlistProcessor;
    }

    public string Name => "Fix Genres SpotDL using SPOTDL";

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

        var stopwatch = Stopwatch.StartNew();
        var playlistPath = Path.Combine(_fileService.GetPlaylistsPath(), playlistName);
        await _playlistProcessor.FixPlaylistGenresUsingSpotdlMetadataAsync(playlistPath);
        Console.WriteLine($"Finished in {stopwatch.Elapsed.Milliseconds}ms");
    }
}