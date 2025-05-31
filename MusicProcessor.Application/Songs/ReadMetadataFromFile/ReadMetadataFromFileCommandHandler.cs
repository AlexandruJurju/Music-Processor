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
    ILogger<ReadMetadataFromFileCommandHandler> logger
) : ICommandHandler<ReadMetadataFromFileCommand>
{
    private readonly Dictionary<string, Album> _albumCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Artist> _artistCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Style> _styleCache = new(StringComparer.OrdinalIgnoreCase);

    public async ValueTask<Result> Handle(ReadMetadataFromFileCommand request, CancellationToken cancellationToken)
    {
        await PreloadCachesAsync();

        List<Song> songs = await spotDlMetadataReader.LoadSpotDLMetadataAsync();

        (List<Artist> newArtists, List<Style> newStyles, List<Album> newAlbums) = ProcessSongDependencies(songs);

        if (newArtists.Count > 0)
        {
            await artistRepository.BulkInsertAsync(newArtists);
            logger.LogInformation("Inserted {Count} new artists", newArtists.Count);
        }

        if (newStyles.Count > 0)
        {
            await styleRepository.BulkInsertAsync(newStyles);
            logger.LogInformation("Inserted {Count} new styles", newStyles.Count);
        }

        if (newAlbums.Count > 0)
        {
            await albumRepository.BulkInsertAsync(newAlbums);
            logger.LogInformation("Inserted {Count} new albums", newAlbums.Count);
        }

        var songsList = songs.ToList();
        await songRepository.BulkInsertAsync(songsList);
        logger.LogInformation("Successfully processed {Count} songs", songsList.Count);

        return Result.Success();
    }

    private async Task PreloadCachesAsync()
    {
        IEnumerable<Artist> existingArtists = await artistRepository.GetAllAsync();
        foreach (Artist artist in existingArtists)
        {
            if (!string.IsNullOrEmpty(artist.Name))
            {
                _artistCache[artist.Name] = artist;
            }
        }
        logger.LogDebug("Preloaded {Count} artists into cache", _artistCache.Count);

        IEnumerable<Style> existingStyles = await styleRepository.GetAllAsync();
        foreach (Style style in existingStyles)
        {
            if (!string.IsNullOrEmpty(style.Name))
            {
                _styleCache[style.Name] = style;
            }
        }
        logger.LogDebug("Preloaded {Count} styles into cache", _styleCache.Count);

        IEnumerable<Album> existingAlbums = await albumRepository.GetAllAsync();
        foreach (Album album in existingAlbums)
        {
            if (!string.IsNullOrEmpty(album.Name))
            {
                _albumCache[album.Name] = album;
            }
        }
        logger.LogDebug("Preloaded {Count} albums into cache", _albumCache.Count);
    }

    private (List<Artist>, List<Style>, List<Album>) ProcessSongDependencies(IEnumerable<Song> songs)
    {
        var newArtists = new List<Artist>();
        var newStyles = new List<Style>();
        var newAlbums = new List<Album>();

        foreach (Song song in songs)
        {
            if (!string.IsNullOrEmpty(song.MainArtist.Name))
            {
                song.MainArtist = GetOrCreateArtistAsync(song.MainArtist, newArtists);
            }

            for (int i = 0; i < song.Artists.Count; i++)
            {
                if (!string.IsNullOrEmpty(song.Artists[i].Name))
                {
                    song.Artists[i] = GetOrCreateArtistAsync(song.Artists[i], newArtists);
                }
            }
            
            if (!string.IsNullOrEmpty(song.Album.MainArtist.Name))
            {
                song.Album.MainArtist = GetOrCreateArtistAsync(song.Album.MainArtist, newArtists);
            }

            if (!string.IsNullOrEmpty(song.Album.Name))
            {
                song.Album = GetOrCreateAlbumAsync(song.Album, newAlbums);
            }

            for (int i = 0; i < song.Styles.Count; i++)
            {
                if (!string.IsNullOrEmpty(song.Styles[i].Name))
                {
                    song.Styles[i] = GetOrCreateStyleAsync(song.Styles[i], newStyles);
                }
            }
        }

        return (newArtists, newStyles, newAlbums);
    }

    private Artist GetOrCreateArtistAsync(Artist artist, List<Artist> newArtists)
    {
        if (_artistCache.TryGetValue(artist.Name, out Artist existingArtist))
        {
            return existingArtist;
        }

        Artist newArtist = newArtists.FirstOrDefault(a =>
            string.Equals(a.Name, artist.Name, StringComparison.OrdinalIgnoreCase));

        if (newArtist != null)
        {
            _artistCache[artist.Name] = newArtist;
            return newArtist;
        }

        _artistCache[artist.Name] = artist;
        newArtists.Add(artist);
        return artist;
    }

    private Album GetOrCreateAlbumAsync(Album album, List<Album> newAlbums)
    {
        if (_albumCache.TryGetValue(album.Name, out Album existingAlbum))
        {
            return existingAlbum;
        }

        Album newAlbum = newAlbums.FirstOrDefault(a =>
            string.Equals(a.Name, album.Name, StringComparison.OrdinalIgnoreCase));

        if (newAlbum != null)
        {
            _albumCache[album.Name] = newAlbum;
            return newAlbum;
        }

        _albumCache[album.Name] = album;
        newAlbums.Add(album);
        return album;
    }

    private Style GetOrCreateStyleAsync(Style style, List<Style> newStyles)
    {
        if (_styleCache.TryGetValue(style.Name, out Style existingStyle))
        {
            return existingStyle;
        }

        Style newStyle = newStyles.FirstOrDefault(s =>
            string.Equals(s.Name, style.Name, StringComparison.OrdinalIgnoreCase));

        if (newStyle != null)
        {
            _styleCache[style.Name] = newStyle;
            return newStyle;
        }

        _styleCache[style.Name] = style;
        newStyles.Add(style);
        return style;
    }
}
