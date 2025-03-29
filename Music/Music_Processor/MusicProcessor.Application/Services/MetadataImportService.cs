using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Domain.SongsMetadata;

namespace MusicProcessor.Application.Services;

public class MetadataImportService : IMetadatImportService
{
    private readonly Dictionary<string, Album> _albumCache = new();
    private readonly IAlbumRepository _albumRepository;

    private readonly Dictionary<string, Artist> _artistCache = new();
    private readonly IArtistRepository _artistRepository;
    
    private readonly Dictionary<string, Genre> _genreCache = new();
    private readonly IGenreRepository _genreRepository;
    
    private readonly ILogger<MetadataImportService> _logger;
    private readonly ISongMetadataRepository _songMetadataRepository;

    public MetadataImportService(
        IArtistRepository artistRepository,
        IGenreRepository genreRepository,
        ISongMetadataRepository songMetadataRepository,
        IAlbumRepository albumRepository,
        ILogger<MetadataImportService> logger)
    {
        _artistRepository = artistRepository;
        _genreRepository = genreRepository;
        _songMetadataRepository = songMetadataRepository;
        _albumRepository = albumRepository;
        _logger = logger;
    }

    public async Task ImportSongMetadataAsync(IEnumerable<SongMetadata> songs)
    {
        await PreloadCachesAsync();

        var songsToAdd = new List<SongMetadata>();
        var newArtists = new List<Artist>();
        var newGenres = new List<Genre>();
        var newAlbums = new List<Album>();

        foreach (var song in songs)
        {
            try
            {
                await ProcessSongAsync(song, newArtists, newGenres, newAlbums);
                songsToAdd.Add(song);
                _logger.LogDebug($"Processed song: {song.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing song {song.Name}: {ex.Message}");
            }
        }

        // Batch insert new entities
        if (newArtists.Any())
            await _artistRepository.AddRangeAsync(newArtists);
        if (newGenres.Any())
            await _genreRepository.AddRangeAsync(newGenres);
        if (newAlbums.Any())
            await _albumRepository.AddRangeAsync(newAlbums);

        // Batch insert songs
        if (songsToAdd.Any())
            await _songMetadataRepository.AddRangeAsync(songsToAdd);

        _logger.LogInformation($"Completed processing {songsToAdd.Count} songs");
    }

    private async Task ProcessSongAsync(
        SongMetadata song,
        List<Artist> newArtists,
        List<Genre> newGenres,
        List<Album> newAlbums)
    {
        await ProcessArtistsAsync(song, newArtists);

        await ProcessAlbumAsync(song, newArtists, newAlbums);

        await ProcessGenresAsync(song, newGenres);
    }

    private async Task ProcessGenresAsync(SongMetadata song, List<Genre> newGenres)
    {
        // Process genres
        var genres = new List<Genre>();
        foreach (var genreMeta in song.Genres)
        {
            var genre = await GetOrCreateGenreBatchStyleAsync(genreMeta, newGenres);
            genres.Add(genre);
        }

        song.Genres = genres;
    }

    private async Task ProcessAlbumAsync(SongMetadata song, List<Artist> newArtists, List<Album> newAlbums)
    {
        // Process album
        if (song.Album != null)
        {
            song.Album.Artist = await GetOrCreateArtistBatchStyleAsync(song.Album.Artist, newArtists);
            song.Album = await GetOrCreateAlbumBatchStyleAsync(song.Album, newAlbums);
        }
    }

    private async Task ProcessArtistsAsync(SongMetadata song, List<Artist> newArtists)
    {
        // Process artists
        var artists = new List<Artist>();
        foreach (var artistMetadata in song.Artists)
        {
            var artist = await GetOrCreateArtistBatchStyleAsync(artistMetadata, newArtists);
            artists.Add(artist);
        }

        song.Artists = artists;

        // Process main artist
        song.MainArtist = await GetOrCreateArtistBatchStyleAsync(song.MainArtist, newArtists);
    }

    private async Task<Artist> GetOrCreateArtistBatchStyleAsync(Artist artist, List<Artist> newArtists)
    {
        if (_artistCache.TryGetValue(artist.Name, out var cachedArtist))
            return cachedArtist;

        var existingArtist = await _artistRepository.GetByNameAsync(artist.Name);
        if (existingArtist != null)
        {
            _artistCache[artist.Name] = existingArtist;
            return existingArtist;
        }

        // Instead of immediate insert, add to batch
        newArtists.Add(artist);
        _artistCache[artist.Name] = artist;
        return artist;
    }

    private async Task<Album> GetOrCreateAlbumBatchStyleAsync(Album album, List<Album> newAlbums)
    {
        if (_albumCache.TryGetValue(album.Name, out var cachedAlbum))
            return cachedAlbum;

        var existingAlbum = await _albumRepository.GetByNameAsync(album.Name);
        if (existingAlbum != null)
        {
            _albumCache[album.Name] = existingAlbum;
            return existingAlbum;
        }

        newAlbums.Add(album);
        _albumCache[album.Name] = album;
        return album;
    }

    private async Task<Genre> GetOrCreateGenreBatchStyleAsync(Genre genre, List<Genre> newGenres)
    {
        if (_genreCache.TryGetValue(genre.Name, out var cachedGenre))
            return cachedGenre;

        var existingGenre = await _genreRepository.GetByNameAsync(genre.Name);
        if (existingGenre != null)
        {
            _genreCache[genre.Name] = existingGenre;
            return existingGenre;
        }

        newGenres.Add(genre);
        _genreCache[genre.Name] = genre;
        return genre;
    }

    private async Task PreloadCachesAsync()
    {
        var allAlbums = await _albumRepository.GetAllAsync();
        foreach (var album in allAlbums)
            _albumCache[album.Name] = album;

        var allArtists = await _artistRepository.GetAllAsync();
        foreach (var artist in allArtists)
            _artistCache[artist.Name] = artist;

        var allGenres = await _genreRepository.GetAllAsync();
        foreach (var genre in allGenres)
            _genreCache[genre.Name] = genre;
    }
}