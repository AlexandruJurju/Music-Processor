using System.CommandLine;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.Domain.Enums;

namespace MusicProcessor.CLI.Commands;

public class FirstTimeSyncCommand(
    ISpotDLService spotDlService,
    IFileService fileService,
    IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override Command CreateSubCommand()
    {
        var command = new Command("sync-new", "First time sync with Spotify playlist");
        var urlOption = new Option<string>("--url", "The Spotify playlist URL") { IsRequired = true };
        var playlistOption = new Option<string>("--playlist", "The playlist name") { IsRequired = true };

        command.AddOption(urlOption);
        command.AddOption(playlistOption);

        command.SetHandler(async (string url, string playlistName) =>
        {
            if (!ValidatePlaylist(playlistName))
            {
                return;
            }

            var playlistPath = GetPlaylistPath(playlistName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSpotDL Started");

            await foreach (var output in spotDlService.NewSyncAsync(url, playlistPath))
            {
                Console.ForegroundColor = output.Type == OutputType.Success ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(output.Data);
                Console.ResetColor();
            }

            Console.WriteLine("\nSpot DL Finished");
            Console.WriteLine("\nAdding new songs to db");
            await Mediator.Send(new WriteLibraryWithSpotdlFileCommand(playlistPath));
        }, urlOption, playlistOption);

        return command;
    }
}