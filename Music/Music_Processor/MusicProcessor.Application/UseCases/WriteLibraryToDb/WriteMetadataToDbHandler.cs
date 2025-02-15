using MediatR;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.WriteLibraryToDb;

public sealed class WriteMetadataToDbHandler(
    IFileService fileService,
    IMetadataService metadataService,
    ISongProcessor songProcessor)
    : IRequestHandler<WriteMetadataToDbCommand>
{
    public async Task Handle(WriteMetadataToDbCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();
        var playlistSongs = fileService.GetAllAudioFilesInFolder(request.PlaylistPath);

        foreach (var songFile in playlistSongs)
        {
            var metadata = metadataService.ReadMetadata(songFile);
            songsToAdd.Add(metadata);
        }

        if (songsToAdd.Any()) await songProcessor.AddRawSongsToDbAsync(songsToAdd);
    }
}