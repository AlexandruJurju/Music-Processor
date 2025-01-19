using MediatR;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.WriteLibraryToDb;

public sealed class WriteMetadataToDbHandler(
    IFileService fileService,
    MetadataHandlerFactory metadataHandlerFactory,
    ISongProcessor songProcessor)
    : IRequestHandler<WriteMetadataToDbCommand>
{
    public async Task Handle(WriteMetadataToDbCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();
        var playlistSongs = fileService.GetAllAudioFilesInFolder(request.PlaylistPath);

        foreach (var songFile in playlistSongs)
        {
            var handler = metadataHandlerFactory.GetHandler(songFile);
            var metadata = handler.ExtractMetadata(songFile);
            songsToAdd.Add(metadata);
        }

        if (songsToAdd.Any())
        {
            await songProcessor.AddRawSongToDbAsync(songsToAdd);
        }
    }
}