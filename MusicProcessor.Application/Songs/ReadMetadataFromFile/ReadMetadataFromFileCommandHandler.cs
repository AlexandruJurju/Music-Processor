﻿using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.ReadMetadataFromFile;

public class ReadMetadataFromFileCommandHandler(
    ISongRepository songRepository,
    IMetadataService metadataService
) : ICommandHandler<ReadMetadataFromFileCommand>
{
    public async ValueTask<Result> Handle(ReadMetadataFromFileCommand request, CancellationToken cancellationToken)
    {
        List<SongMetadata> songsMetadata = await metadataService.ReadSpotDlMetadataAsync();
        
        var songs = songsMetadata.Select(metadata =>
            Song.Create(
                title: metadata.Name,
                mainArtistName: metadata.Artist,
                artists: metadata.Artists?.ToList(),
                styles: metadata.Styles?.ToList(),
                albumName: metadata.AlbumName,
                discNumber: metadata.DiscNumber,
                discCount: metadata.DiscCount,
                duration: metadata.Duration,
                year: metadata.Year,
                trackNumber: metadata.TrackNumber,
                tracksCount: metadata.TracksCount,
                isrc: metadata.ISRC
            )
        ).ToList();
        
        await songRepository.AddRangeAsync(songs);
        
        return Result.Success();
    }
}
