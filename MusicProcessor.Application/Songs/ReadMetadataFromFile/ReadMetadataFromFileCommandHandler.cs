using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Abstractions.Messaging;
using MusicProcessor.Domain.Abstractions.Persistence;
using MusicProcessor.Domain.Abstractions.Result;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Application.Songs.ReadMetadataFromFile;

public class ReadMetadataFromFileCommandHandler(
    ISongRepository songRepository,
    IArtistRepository artistRepository,
    IStyleRepository styleRepository,
    IAlbumRepository albumRepository,
    ISpotDLMetadataReader spotDlMetadataReader
) : ICommandHandler<ReadMetadataFromFileCommand>
{
    public async ValueTask<Result> Handle(ReadMetadataFromFileCommand request, CancellationToken cancellationToken)
    {
        List<Song> spotDlSongs = await spotDlMetadataReader.LoadSpotDLMetadataAsync();

        var allArtists = spotDlSongs
            .Select(s => s.MainArtist)
            .Concat(spotDlSongs.SelectMany(s => s.Artists))
            .Concat(spotDlSongs.Select(s => s.Album.MainArtist))
            .DistinctBy(a => a.Name.ToUpperInvariant())
            .ToList();
        await artistRepository.BulkInsertAsync(allArtists);

        var allStyles = spotDlSongs
            .SelectMany(s => s.Styles)
            .DistinctBy(s => s.Name.ToUpperInvariant())
            .ToList();
        await styleRepository.BulkInsertAsync(allStyles);

        var allAlbums = spotDlSongs
            .Select(s => s.Album)
            .DistinctBy(a => a.Name.ToUpperInvariant())
            .ToList();
        await albumRepository.BulkInsertAsync(allAlbums);

        IEnumerable<Song> badSongs = spotDlSongs.Where(e => e.Album.Id == 0);
        Console.WriteLine(badSongs);
        
        await songRepository.BulkInsertAsync(spotDlSongs.Where(e => e.Album.Id != 0));

        return Result.Success();
    }
}
