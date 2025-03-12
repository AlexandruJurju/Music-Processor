﻿using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.MenuCommands;

[Command("sync-new", Description = "First time sync with Spotify playlist")]
public class FirstTimeSyncMenuCommand : BaseMenuCommand
{
    private readonly ISpotDLService _spotDlService;

    public FirstTimeSyncMenuCommand(IFileService fileService, IMediator mediator, ISpotDLService spotDlService) : base(fileService, mediator)
    {
        _spotDlService = spotDlService;
    }

    [CommandOption("url", 'u', IsRequired = true, Description = "The Spotify playlist URL")]
    public string Url { get; init; } = "";

    [CommandOption("playlist", 'p', IsRequired = true, Description = "The playlist name")]
    public string PlaylistName { get; init; } = "";

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        if (!ValidatePlaylist(PlaylistName, console)) return;

        var playlistPath = GetPlaylistPath(PlaylistName);

        console.Output.WriteLine("\nSpotDL Started");

        await foreach (var output in _spotDlService.NewSyncAsync(Url, playlistPath))
        {
            var color = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
            console.Output.WriteLine(output.Data, color);
        }

        await console.Output.WriteLineAsync("\nSpot DL Finished");
        await console.Output.WriteLineAsync("\nAdding new songs to db");
        await _mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
    }
}