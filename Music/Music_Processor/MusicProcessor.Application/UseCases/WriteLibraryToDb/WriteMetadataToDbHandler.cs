using MediatR;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.UseCases.WriteLibraryToDb;

public sealed class WriteMetadataToDbHandler : IRequestHandler<WriteMetadataToDbCommand>
{
    private readonly IFileService _fileService;
    private readonly IMetadataService _metadataService;
    private readonly ISongProcessor _songProcessor;

    public WriteMetadataToDbHandler(IFileService fileService,
        IMetadataService metadataService,
        ISongProcessor songProcessor)
    {
        _fileService = fileService;
        _metadataService = metadataService;
        _songProcessor = songProcessor;
    }

    public async Task Handle(WriteMetadataToDbCommand request, CancellationToken cancellationToken)
    {
        var songsToAdd = new List<Song>();
        var playlistSongs = _fileService.GetAllAudioFilesInFolder(request.PlaylistPath);

        foreach (var songFile in playlistSongs)
        {
            var metadata = _metadataService.ReadMetadata(songFile);
            songsToAdd.Add(metadata);
        }

        if (songsToAdd.Any()) await _songProcessor.AddRawSongsToDbAsync(songsToAdd);
    }
}