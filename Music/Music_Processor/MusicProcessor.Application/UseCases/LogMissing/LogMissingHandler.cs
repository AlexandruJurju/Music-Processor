using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using Serilog;

namespace MusicProcessor.Application.UseCases.LogMissing;

public class LogMissingHandler : IRequestHandler<LogMissingQuery>
{
    private readonly IFileService _fileService;
    private readonly ILogger<LogMissingHandler> _logger;
    private readonly IMetadataService _metadataService;
    private readonly ISongMetadataRepository _songMetadataRepository;

    public LogMissingHandler(ISongMetadataRepository songMetadataRepository, IFileService fileService, IMetadataService metadataService, ILogger<LogMissingHandler> logger)
    {
        _songMetadataRepository = songMetadataRepository;
        _fileService = fileService;
        _metadataService = metadataService;
        _logger = logger;
    }

    public async Task Handle(LogMissingQuery request, CancellationToken cancellationToken)
    {
        var playlistMetadata = await _songMetadataRepository.GetAllSongsWithKeyAsync();
        var songFiles = _fileService.GetAllMainMusicFiles();

        foreach (var songFile in songFiles)
        {
            try
            {
                var songMetadata = _metadataService.ReadMetadata(songFile);
                if (!playlistMetadata.TryGetValue(songMetadata.Key, out var metadata))
                {
                    Log.Error("Unable to find song metadata for \n{Key}\n{Name}", songMetadata.Key, songMetadata.Name);
                    _logger.LogError("Unable to find song metadata for \n{Key}\n{Name}", songMetadata.Key, songMetadata.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to read song metadata for \n{songFile}\n{ex.Message}");
            }
        }
    }
}