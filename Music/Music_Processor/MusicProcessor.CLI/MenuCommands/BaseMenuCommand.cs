using CliFx.Infrastructure;
using MediatR;
using MusicProcessor.Application.Interfaces.Infrastructure;
using ICommand = CliFx.ICommand;

namespace MusicProcessor.CLI.MenuCommands;

public abstract class BaseMenuCommand : ICommand
{
    private readonly IFileService _fileService;
    protected readonly IMediator _mediator;

    protected BaseMenuCommand(
        IFileService fileService,
        IMediator mediator
    )
    {
        _fileService = fileService;
        _mediator = mediator;
    }

    public abstract ValueTask ExecuteAsync(IConsole console);

    protected IEnumerable<string> GetAvailablePlaylists()
    {
        return _fileService.GetAllSpotDLPlaylistsNames().Append(_fileService.GetMainMusicFolderPath());
    }
}