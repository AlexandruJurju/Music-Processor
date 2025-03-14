using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Application.UseCases.CommitChanges;

public sealed class CommitChangesHandler : IRequestHandler<CommitChangesCommand>
{
    private readonly ILogger<CommitChangesHandler> _logger;
    private readonly IMetadataService _metadataService;
    private readonly ISongRepository _songRepository;

    public CommitChangesHandler(ISongRepository songRepository,
        IMetadataService metadataService,
        ILogger<CommitChangesHandler> logger)
    {
        _songRepository = songRepository;
        _metadataService = metadataService;
        _logger = logger;
    }

    public async Task Handle(CommitChangesCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("CommitChanges started at {StartTime}", startTime);

        var songs = await _songRepository.GetAllAsync();

        foreach (var song in songs)
        {
            _metadataService.WriteMetadata(song);
        }

        var endTime = DateTime.UtcNow;
        _logger.LogInformation("CommitChanges completed at {EndTime}, total duration: {TotalMilliseconds} ms", endTime, (endTime - startTime).TotalSeconds);
    }
}