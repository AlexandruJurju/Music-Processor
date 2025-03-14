using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.Services;

public class SongProcessor : ISongProcessor
{
    private readonly IArtistRepository _artistRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ISongRepository _songRepository;
    private readonly IAlbumRepository _albumRepository;
    private readonly ILogger<SongProcessor> _logger;

    public SongProcessor(
        IArtistRepository artistRepository,
        IGenreRepository genreRepository,
        ISongRepository songRepository,
        IAlbumRepository albumRepository,
        ILogger<SongProcessor> logger)
    {
        _artistRepository = artistRepository;
        _genreRepository = genreRepository;
        _songRepository = songRepository;
        _albumRepository = albumRepository;
        _logger = logger;
    }

    public async Task AddMetadataToDbAsync(IEnumerable<Song> songs)
    {
        var songsList = songs.ToList();
        _logger.LogInformation($"Processing {songsList.Count} songs");

        // Load existing entities from the database
        var existingArtists = await _artistRepository.GetAllAsync();
        var existingGenres = await _genreRepository.GetAllAsync();
        var existingAlbums = await _albumRepository.GetAllAsync();

        _logger.LogDebug($"Loaded entities: {existingArtists.Count} artists, {existingGenres.Count} genres, {existingAlbums.Count} albums");

        // Declare lists to collect new entities
        var newArtists = new List<Artist>();
        var newGenres = new List<Genre>();
        var newAlbums = new List<Album>();

        foreach (var song in songsList)
        {
            try
            {
                _logger.LogInformation($"Processing song: {song.Name} by {song.MainArtist.Name}");

                ProcessArtists(song, existingArtists, newArtists);
                ProcessAlbums(song, existingAlbums, existingArtists, newAlbums, newArtists);
                ProcessGenres(song, existingGenres, newGenres);
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
        await _songRepository.AddRangeAsync(songsList);

        _logger.LogDebug("Successfully processed {Count} songs", songsList.Count);
    }

    private void ProcessArtists(Song song, List<Artist> existingArtists, List<Artist> newArtists)
    {
        var artists = song.Artists.ToList();
        song.Artists.Clear();

        foreach (var artist in artists)
        {
            var existingArtist = existingArtists
                .FirstOrDefault(a => a.Name.Equals(artist.Name, StringComparison.OrdinalIgnoreCase));

            if (existingArtist != null)
            {
                song.Artists.Add(existingArtist);
                _logger.LogDebug("Using existing artist: {ArtistName}", artist.Name);
            }
            else
            {
                newArtists.Add(artist);
                existingArtists.Add(artist);
                song.Artists.Add(artist);
                _logger.LogDebug("Added new artist: {ArtistName}", artist.Name);
            }
        }

        var existingMainArtist = existingArtists.FirstOrDefault(a => a.Name.Equals(song.MainArtist.Name, StringComparison.OrdinalIgnoreCase));
        if (existingMainArtist != null)
        {
            song.MainArtist = existingMainArtist;
            _logger.LogDebug("Using existing artist: {ArtistName}", existingMainArtist.Name);
        }
        else
        {
            newArtists.Add(song.MainArtist);
            existingArtists.Add(song.MainArtist);
            song.MainArtist = song.MainArtist;
            _logger.LogDebug("Added new artist: {ArtistName}", song.MainArtist.Name);
        }
    }

    private void ProcessAlbums(Song song, List<Album> existingAlbums, List<Artist> existingArtists, List<Album> newAlbums, List<Artist> newArtists)
    {
        if (song.Album == null) return;

        var existingAlbum = existingAlbums
            .FirstOrDefault(a => a.Name.Equals(song.Album.Name, StringComparison.OrdinalIgnoreCase));

        if (existingAlbum != null)
        {
            song.Album = existingAlbum;
            _logger.LogDebug("Using existing album: {AlbumName}", existingAlbum.Name);
        }
        else
        {
            var albumArtist = song.Album.Artist;
            var existingAlbumArtist = existingArtists.FirstOrDefault(a => a.Name.Equals(albumArtist.Name, StringComparison.OrdinalIgnoreCase));
            if (existingAlbumArtist != null)
            {
                song.Album.Artist = existingAlbumArtist;
            }
            else
            {
                newArtists.Add(albumArtist);
                existingArtists.Add(albumArtist);
            }

            newAlbums.Add(song.Album);
            existingAlbums.Add(song.Album);
            _logger.LogDebug("Added new album: {AlbumName}", song.Album.Name);
        }
    }

    private void ProcessGenres(Song song, List<Genre> existingGenres, List<Genre> newGenres)
    {
        var genres = song.Genres.ToList();
        song.Genres.Clear();

        foreach (var genre in genres)
        {
            var existingGenre = existingGenres.FirstOrDefault(g => g.Name.Equals(genre.Name, StringComparison.OrdinalIgnoreCase));
            if (existingGenre != null)
            {
                song.Genres.Add(existingGenre);
                _logger.LogDebug("Using existing genre: {GenreName}", genre.Name);
            }
            else
            {
                newGenres.Add(genre);
                existingGenres.Add(genre);
                song.Genres.Add(genre);
                _logger.LogDebug("Added new genre: {GenreName}", genre.Name);
            }
        }
    }
}