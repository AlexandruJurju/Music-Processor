using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;

namespace MusicProcessor.Application.UseCases.CommitChangesToLibrary;

public sealed class CommitChangesToLibraryHandler(
    ISongRepository songRepository,
    IMetadataService metadataService,
    ILogger<CommitChangesToLibraryHandler> logger
) : IRequestHandler<CommitChangesToLibraryCommand>
{
    public async Task Handle(CommitChangesToLibraryCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        logger.LogInformation("CommitChanges started at {StartTime}", startTime);

        var songs = await songRepository.GetAllAsync();

        foreach (var song in songs)
        {
            metadataService.WriteMetadata(song);
        }

        var endTime = DateTime.UtcNow;
        logger.LogInformation("CommitChanges completed at {EndTime}, total duration: {TotalMilliseconds} ms", endTime, (endTime - startTime).TotalSeconds);
    }
}