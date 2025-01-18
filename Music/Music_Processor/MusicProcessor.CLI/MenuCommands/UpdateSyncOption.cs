using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.SyncCommand;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuCommands;

public class UpdateSyncOption : IMenuOption
{
    private readonly IFileService _fileService;
    private readonly ISpotDLService _spotDLService;
    private readonly IMediator _mediator;


    public UpdateSyncOption(ISpotDLService spotDlService, IFileService fileService, IMediator mediator)
    {
        _spotDLService = spotDlService;
        _fileService = fileService;
        _mediator = mediator;
    }

    public string Name => "SPOTDL: Update Sync";

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
        var playlistFolders = availablePlaylists.Select(path => Path.GetFileName(path)).ToList();
        if (!playlistFolders.Contains(playlistName))
        {
            Console.WriteLine("Playlist does not exist.");
            return;
        }

        // check if sync file exists
        var syncFile = Path.Combine(_fileService.GetPlaylistsPath(), playlistName, $"{playlistName}.spotdl");
        if (string.IsNullOrEmpty(syncFile))
        {
            Console.WriteLine("No spotdl file found.");
        }

        var playlistDirPath = Path.Combine(_fileService.GetPlaylistsPath(), playlistName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nSpotDL Started");

        await foreach (var output in _spotDLService.UpdateSyncAsync(playlistDirPath))
        {
            Console.ForegroundColor = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(output.Data);
            Console.ResetColor();
        }

        Console.WriteLine("\nSpot DL Finished");
        Console.WriteLine("\nAdding new songs to db");
        await _mediator.Send(new SyncDbCommand(playlistDirPath));
    }
}