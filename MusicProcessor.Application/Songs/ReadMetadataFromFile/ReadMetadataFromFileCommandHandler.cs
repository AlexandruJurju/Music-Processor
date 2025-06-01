using System.Globalization;
using Microsoft.Extensions.Logging;
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
    ISpotDLMetadataReader spotDlMetadataReader,
    IUnitOfWork unitOfWork,
    ILogger<ReadMetadataFromFileCommandHandler> logger
) : ICommandHandler<ReadMetadataFromFileCommand>
{
    private readonly Dictionary<string, Album> _albumCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Artist> _artistCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Style> _styleCache = new(StringComparer.OrdinalIgnoreCase);

    public async ValueTask<Result> Handle(ReadMetadataFromFileCommand request, CancellationToken cancellationToken)
    {
        await PreloadCachesAsync();

        List<SpotDLSongMetadata> songs = await spotDlMetadataReader.LoadSpotDLMetadataAsync();

        (List<Artist> newArtists, List<Style> newStyles, List<Album> newAlbums, List<Song> newSongs) = ProcessSongDependencies(songs);

        if (newArtists.Count > 0)
        {
            artistRepository.AddRange(newArtists);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Inserted {Count} new artists", newArtists.Count);
        }

        if (newStyles.Count > 0)
        {
            styleRepository.AddRange(newStyles);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Inserted {Count} new styles", newStyles.Count);
        }

        if (newAlbums.Count > 0)
        {
            albumRepository.AddRange(newAlbums);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Inserted {Count} new albums", newAlbums.Count);
        }

        if (newSongs.Count > 0)
        {
            songRepository.AddRange(newSongs);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully processed {Count} songs", newSongs.Count);
        }


        return Result.Success();
    }

    private async Task PreloadCachesAsync()
    {
        IEnumerable<Artist> existingArtists = await artistRepository.GetAllAsync();
        foreach (Artist artist in existingArtists)
        {
            _artistCache[artist.Name] = artist;
        }

        logger.LogDebug("Preloaded {Count} artists into cache", _artistCache.Count);

        IEnumerable<Style> existingStyles = await styleRepository.GetAllAsync();
        foreach (Style style in existingStyles)
        {
            _styleCache[style.Name] = style;
        }

        logger.LogDebug("Preloaded {Count} styles into cache", _styleCache.Count);

        IEnumerable<Album> existingAlbums = await albumRepository.GetAllAsync();
        foreach (Album album in existingAlbums)
        {
            _albumCache[album.Name] = album;
        }

        logger.LogDebug("Preloaded {Count} albums into cache", _albumCache.Count);
    }

    private (List<Artist>, List<Style>, List<Album>, List<Song>) ProcessSongDependencies(IEnumerable<SpotDLSongMetadata> songs)
    {
        var newArtists = new List<Artist>();
        var newStyles = new List<Style>();
        var newAlbums = new List<Album>();
        var newSongs = new List<Song>();

        foreach (SpotDLSongMetadata song in songs)
        {
            Artist mainArtist = GetOrCreateArtistAsync(song.Artist, newArtists);

            var otherArtists = song.Artists.Select(e => GetOrCreateArtistAsync(e, newArtists)).ToList();

            Artist albumArtist = GetOrCreateArtistAsync(song.AlbumArtist, newArtists);
            Album album = GetOrCreateAlbumAsync(song.AlbumName, albumArtist, newAlbums);

            var styles = song.Genres.Select(e => GetOrCreateStyleAsync(e, newStyles)).ToList();

            newSongs.Add(Song.Create(
                song.Name,
                mainArtist,
                otherArtists,
                styles,
                album,
                song.DiscNumber,
                song.DiscCount,
                song.Duration,
                uint.Parse(song.Year, CultureInfo.InvariantCulture),
                song.TrackNumber,
                song.TracksCount,
                song.ISRC
            ));
        }

        return (newArtists, newStyles, newAlbums, newSongs);
    }

    private Artist GetOrCreateArtistAsync(string artistName, List<Artist> newArtists)
    {
        if (_artistCache.TryGetValue(artistName, out Artist existingArtist))
        {
            return existingArtist;
        }

        var newArtist = Artist.Create(artistName);

        _artistCache[artistName] = newArtist;
        newArtists.Add(newArtist);
        return newArtist;
    }

    private Album GetOrCreateAlbumAsync(string albumName, Artist artist, List<Album> newAlbums)
    {
        if (_albumCache.TryGetValue(albumName, out Album existingAlbum))
        {
            return existingAlbum;
        }

        var newAlbum = Album.Create(albumName, artist);

        _albumCache[albumName] = newAlbum;
        newAlbums.Add(newAlbum);
        return newAlbum;
    }

    private Style GetOrCreateStyleAsync(string styleName, List<Style> newStyles)
    {
        if (_styleCache.TryGetValue(styleName, out Style existingStyle))
        {
            return existingStyle;
        }

        var newStyle = Style.Create(styleName, false);

        _styleCache[styleName] = newStyle;
        newStyles.Add(newStyle);
        return newStyle;
    }
}
