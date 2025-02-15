using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using TagLib;
using File = System.IO.File;

namespace MusicProcessor.Application.UseCases.CommitChangesToLibrary;

public sealed class CommitChangesToLibraryHandler(
    ISongRepository songRepository,
    IMetadataService metadataService,
    ILogger<CommitChangesToLibraryHandler> logger
) : IRequestHandler<CommitChangesToLibraryCommand>
{
    public async Task Handle(CommitChangesToLibraryCommand request, CancellationToken cancellationToken)
    {
        var songs = await songRepository.GetAllAsync();

        foreach (var song in songs)
        {
            // var lastCommitedDate = File.GetLastWriteTime(song.FilePath);
            // if (song.DateModified > lastCommitedDate)
            // {
            //     logger.LogInformation("{Message}", $"Skipping writing song {song.Title}, it has the latest changes");
            //     continue;
            // }

            metadataService.WriteMetadata(song);
        }

        logger.LogInformation("{Message}", "CommitChanges completed successfully");
    }
}