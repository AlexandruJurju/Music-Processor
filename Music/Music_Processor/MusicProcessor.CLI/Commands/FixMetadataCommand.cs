using System.CommandLine;
using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;

namespace MusicProcessor.CLI.Commands;

public sealed class FixMetadataCommand(IFileService fileService, IMediator mediator) : BaseCommand(fileService, mediator)
{
    public override Command CreateSubCommand()
    {
        var command = new Command("fix-metadata", "Fix metadata for a playlist");

        var availablePlaylists = FileService.GetAllPlaylistsNames();

        var playlistOption = new Option<string>(
            name: "--playlist",
            description: "Name of the playlist to process")
        {
            IsRequired = true
        }.FromAmong(availablePlaylists!);

        command.AddOption(playlistOption);
        command.SetHandler(async (playlistName) =>
        {
            var playlistPath = GetPlaylistPath(playlistName);
            await Mediator.Send(new Application.UseCases.FixMetadata.FixMetadataCommand(playlistPath));
        }, playlistOption);

        return command;
    }
}