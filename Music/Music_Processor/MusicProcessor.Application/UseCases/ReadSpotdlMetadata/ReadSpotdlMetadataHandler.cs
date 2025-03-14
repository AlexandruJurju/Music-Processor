using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

public class ReadSpotdlMetadataHandler : IRequestHandler<ReadSpotdlMetadataCommand>
{
    private readonly ISpotDLMetadataReader _spotDlMetadataReader;
    private readonly IFileService _fileService;
    private readonly ISongRepository _songRepository;
    private readonly IMetadataService _metadataService;
    private readonly ISongProcessor _songProcessor;
    private readonly ILogger<ReadSpotdlMetadataHandler> _logger;

    public ReadSpotdlMetadataHandler(
        ISpotDLMetadataReader spotDlMetadataReader,
        IFileService fileService,
        ISongRepository songRepository,
        ILogger<ReadSpotdlMetadataHandler> logger, IMetadataService metadataService, ISongProcessor songProcessor)
    {
        _spotDlMetadataReader = spotDlMetadataReader;
        _fileService = fileService;
        _songRepository = songRepository;
        _logger = logger;
        _metadataService = metadataService;
        _songProcessor = songProcessor;
    }

    public async Task Handle(ReadSpotdlMetadataCommand request, CancellationToken cancellationToken)
    {
        var spotdlMetadata = await _spotDlMetadataReader.LoadSpotDLMetadataAsync(request.PlaylistName);
        var existingSongs = await _songRepository.GetSongsWithKeyAsync();

        var songsToAdd = new List<Song>();
        foreach (var spotdlSong in spotdlMetadata)
        {
            var key = spotdlSong.Key;
            if (!existingSongs.ContainsKey(key))
            {
                songsToAdd.Add(spotdlSong);
            }
            else
            {
                _logger.LogWarning($"Found song with key {key}");
            }
        }

        await _songProcessor.AddMetadataToDbAsync(songsToAdd);
    }
}