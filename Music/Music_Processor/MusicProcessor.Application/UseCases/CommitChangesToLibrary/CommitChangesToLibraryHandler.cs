using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Services;

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
            var lastCommitedDate = File.GetLastWriteTime(song.FilePath);
            if (song.DateModified > lastCommitedDate)
            {
                logger.LogInformation("{Message}", $"Skipping writing song {song.Title}, it has the latest changes");
                continue;
            }

            try
            {
                metadataService.WriteMetadata(song);
            }
            catch (TagLib.CorruptFileException ex)
            {
                logger.LogError(ex, "Corrupt file detected: {FilePath}", song.FilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Access denied to file: {FilePath}", song.FilePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update metadata for file: {FilePath}", song.FilePath);
            }
        }

        logger.LogInformation("{Message}", "CommitChangesToLibrary completed successfully");
    }
}