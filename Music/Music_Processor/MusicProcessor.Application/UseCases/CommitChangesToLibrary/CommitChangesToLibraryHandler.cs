using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Services;

namespace MusicProcessor.Application.UseCases.CommitChangesToLibrary;

public sealed class CommitChangesToLibraryHandler(
    ISongRepository songRepository,
    MetadataService metadataService
) : IRequestHandler<CommitChangesToLibraryCommand>
{
    public async Task Handle(CommitChangesToLibraryCommand request, CancellationToken cancellationToken)
    {
        var songs = await songRepository.GetAllAsync();

        foreach (var song in songs)
        {
            // todo: optimize later
            // var lastCommitedDate = File.GetLastWriteTime(song.FilePath);

            var strategy = metadataService.GetStrategy(song.FilePath);
            strategy.UpdateMetadata(song);
        }
    }
}