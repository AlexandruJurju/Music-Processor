using CliFx.Attributes;
using CliFx.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Infrastructure.Persistence;

namespace MusicProcessor.CLI.MenuCommands;

[Command("log-missing")]
public class TestCommand : BaseMenuCommand
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<TestCommand> _logger;
    private readonly ISpotDLMetadataLoader _spotDlMetadataLoader;
    private readonly IMetadataService _metadataService;

    public TestCommand(ApplicationDbContext db,
        IFileService fileService,
        IMediator mediator,
        ILogger<TestCommand> logger,
        ISpotDLMetadataLoader spotDlMetadataLoader,
        IMetadataService metadataService) : base(fileService, mediator)
    {
        _db = db;
        _logger = logger;
        _spotDlMetadataLoader = spotDlMetadataLoader;
        _metadataService = metadataService;
    }

    [CommandOption("playlist", 'p', IsRequired = true, Description = "Name of the playlist to process")]
    public required string PlaylistName { get; init; }

    public override async ValueTask ExecuteAsync(IConsole console)
    {
        var audioFiles = _fileService.GetAllAudioFilesInPlaylist(PlaylistName);

        var allSongsPaths = await _db.Songs
            .Select(x => x.FilePath)
            .ToHashSetAsync();

        int index = 1;
        foreach (var file in audioFiles)
        {
            if (!allSongsPaths.Contains(file))
            {
                var metadata = _metadataService.ReadMetadata(file);
                var key = _spotDlMetadataLoader.CreateLookupKey(metadata.MainArtist, metadata.Title);
                _logger.LogError("{Message}", $"{index++}:\n{file},\n{key} \n{string.Join(", ", metadata.Artists.Select(x => x.Name))}\n");
            }
        }
    }
}