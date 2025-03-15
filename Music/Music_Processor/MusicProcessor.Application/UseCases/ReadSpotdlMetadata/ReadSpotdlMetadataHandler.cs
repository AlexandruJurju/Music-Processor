using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.SongsMetadata;

namespace MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

public class ReadSpotdlMetadataHandler : IRequestHandler<ReadSpotdlMetadataCommand>
{
    private readonly ISpotDLMetadataReader _spotDlMetadataReader;
    private readonly IFileService _fileService;
    private readonly ISongMetadataRepository _songMetadataRepository;
    private readonly IMetadataService _metadataService;
    private readonly ISongProcessor _songProcessor;
    private readonly ILogger<ReadSpotdlMetadataHandler> _logger;

    public ReadSpotdlMetadataHandler(
        ISpotDLMetadataReader spotDlMetadataReader,
        IFileService fileService,
        ISongMetadataRepository songMetadataRepository,
        ILogger<ReadSpotdlMetadataHandler> logger, IMetadataService metadataService, ISongProcessor songProcessor)
    {
        _spotDlMetadataReader = spotDlMetadataReader;
        _fileService = fileService;
        _songMetadataRepository = songMetadataRepository;
        _logger = logger;
        _metadataService = metadataService;
        _songProcessor = songProcessor;
    }

    public async Task Handle(ReadSpotdlMetadataCommand request, CancellationToken cancellationToken)
    {
        var spotdlMetadata = await _spotDlMetadataReader.LoadSpotDLMetadataAsync(request.PlaylistName);
        var existingSongs = await _songMetadataRepository.GetAllSongsWithKeyAsync();

        var songsToAdd = new List<SongMetadata>();
        foreach (var spotdlSong in spotdlMetadata)
        {
            if (!existingSongs.ContainsKey(spotdlSong.Key))
            {
                songsToAdd.Add(spotdlSong.Value);
            }
            else
            {
                _logger.LogWarning($"Found song with key {spotdlSong.Key}");
            }
        }

        await _songProcessor.AddMetadataToDbAsync(songsToAdd);
    }
}