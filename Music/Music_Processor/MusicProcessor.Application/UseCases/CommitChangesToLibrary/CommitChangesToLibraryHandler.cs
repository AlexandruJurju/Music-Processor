using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Factories;

namespace MusicProcessor.Application.UseCases.CommitChangesToLibrary;

public sealed class CommitChangesToLibraryHandler(
    MetadataHandlerFactory metadataHandlerFactory,
    ISongRepository songRepository
) : IRequestHandler<CommitChangesToLibraryCommand>
{
    public async Task Handle(CommitChangesToLibraryCommand request, CancellationToken cancellationToken)
    {
        var songs = await songRepository.GetAllAsync();

        foreach (var song in songs)
        {
            // todo: optimize later
            // var lastCommitedDate = File.GetLastWriteTime(song.FilePath);

            var handler = metadataHandlerFactory.GetHandler(song.FilePath);
            handler.UpdateMetadata(song);
        }
    }
}