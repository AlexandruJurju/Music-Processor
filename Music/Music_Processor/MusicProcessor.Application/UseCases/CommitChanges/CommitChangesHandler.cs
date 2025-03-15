using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Application.UseCases.CommitChanges;

public sealed class CommitChangesHandler : IRequestHandler<CommitChangesCommand>
{
    private readonly IMetadataService _metadataService;
    private readonly ISongMetadataRepository _songMetadataRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<CommitChangesHandler> _logger;

    public CommitChangesHandler(
        ISongMetadataRepository songMetadataRepository,
        IMetadataService metadataService,
        IFileService fileService, ILogger<CommitChangesHandler> logger)
    {
        _songMetadataRepository = songMetadataRepository;
        _metadataService = metadataService;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task Handle(CommitChangesCommand request, CancellationToken cancellationToken)
    {
        var songsMetadata = await _songMetadataRepository.GetAllSongsWithKeyAsync();
        var songs = _fileService.GetAllMainMusicFiles().ToList();

        int count = 1;
        foreach (var song in songs)
        {
            try
            {
                var songMetadata = _metadataService.ReadMetadata(song);
                if (songsMetadata.TryGetValue(songMetadata.Key, out var metadata))
                {
                    _metadataService.WriteMetadata(metadata, song);
                    _logger.LogInformation($"Written song {count++}/{songs.Count}: {songMetadata.Key}");
                }
                else
                {
                    count++;
                    _logger.LogError($"Song with key {songMetadata.Key}\n{song}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _logger.LogError($"Song\n{song}");
            }
        }
    }
}