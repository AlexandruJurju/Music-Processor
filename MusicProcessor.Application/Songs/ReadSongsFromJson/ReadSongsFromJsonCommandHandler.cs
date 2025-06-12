using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;

namespace MusicProcessor.Application.Songs.ReadSongsFromJson;

public class ReadSongsFromJsonCommandHandler(
    IMetadataService metadataService,
    ISongRepository songRepository
) : ICommandHandler<ReadSongsFromJsonCommand>
{
    public async ValueTask<Result> Handle(ReadSongsFromJsonCommand request, CancellationToken cancellationToken)
    {
        List<SongMetadata> songsMetadata = await metadataService.ReadFromJsonAsync();

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
