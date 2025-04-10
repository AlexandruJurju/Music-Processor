using MediatR;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.UseCases.CommitChanges;

internal sealed class CommitChangesHandler(
    ISongMetadataRepository songMetadataRepository,
    IMetadataService metadataService,
    IFileService fileService) : IRequestHandler<CommitChangesCommand>
{
    public async Task Handle(CommitChangesCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, SongMetadata> songsMetadata = await songMetadataRepository.GetAllSongsWithKeyAsync();
        var songs = fileService.GetAllMainMusicFiles().ToList();

        foreach (string song in songs)
        {
            SongMetadata songMetadata = metadataService.ReadMetadata(song);
            if (songsMetadata.TryGetValue(songMetadata.Key, out SongMetadata? metadata))
            {
                metadataService.WriteMetadata(metadata, song);
            }
        }
    }
}
