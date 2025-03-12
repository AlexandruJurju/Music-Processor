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

    public async Task AddRawSongsToDbAsync(IEnumerable<Song> songs)
    {
        var songsList = songs.ToList();
        _logger.LogInformation("Processing {Count} songs", songsList.Count);

        // Load existing entities from the database
        var existingArtists = await _artistRepository.GetAllAsync();
        var existingGenres = await _genreRepository.GetAllAsync();
        var existingAlbums = await _albumRepository.GetAllAsync();

        _logger.LogInformation(
            "Loaded entities: {ArtistsCount} artists, {GenresCount} genres, {AlbumsCount} albums",
            existingArtists.Count, existingGenres.Count, existingAlbums.Count);

        // Process each song
        foreach (var song in songsList)
        {
            try
            {
                _logger.LogInformation("Processing song: {SongName} by {Artists}", song.Name, string.Join(", ", song.Artists.Select(a => a.Name)));

                await ProcessArtistsAsync(song, existingArtists);
                await ProcessAlbumsAsync(song, existingAlbums, existingArtists);
                await ProcessGenresAsync(song, existingGenres);

                await _songRepository.AddAsync(song);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing song {song.Name}: {ex.Message}");
            }
        }

        _logger.LogInformation("Successfully processed {Count} songs", songsList.Count);
    }

    private async Task ProcessArtistsAsync(Song song, List<Artist> existingArtists)
    {
        var artists = song.Artists.ToList();
        song.Artists.Clear();

        foreach (var artist in artists)
        {
            var existingArtist = existingArtists.FirstOrDefault(a => a.Name.Equals(artist.Name, StringComparison.OrdinalIgnoreCase));
            if (existingArtist != null)
            {
                song.Artists.Add(existingArtist);
                _logger.LogDebug("Using existing artist: {ArtistName}", artist.Name);
            }
            else
            {
                await _artistRepository.AddAsync(artist);
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
            await _artistRepository.AddAsync(song.MainArtist);
            existingArtists.Add(song.MainArtist);
            song.MainArtist = song.MainArtist;
            _logger.LogDebug("Added new artist: {ArtistName}", song.MainArtist.Name);
        }
    }

    private async Task ProcessAlbumsAsync(Song song, List<Album> existingAlbums, List<Artist> existingArtists)
    {
        if (song.Album == null) return;

        var existingAlbum = existingAlbums.FirstOrDefault(a => a.Name.Equals(song.Album.Name, StringComparison.OrdinalIgnoreCase));
        if (existingAlbum != null)
        {
            song.Album = existingAlbum;
            _logger.LogDebug("Using existing album: {AlbumName}", existingAlbum.Name);
        }
        else
        {
            // Ensure the album's artist is processed
            var albumArtist = song.Album.Artist;
            var existingAlbumArtist = existingArtists.FirstOrDefault(a => a.Name.Equals(albumArtist.Name, StringComparison.OrdinalIgnoreCase));
            if (existingAlbumArtist != null)
            {
                song.Album.Artist = existingAlbumArtist;
            }
            else
            {
                // Add the new artist to the database
                await _artistRepository.AddAsync(albumArtist);
                existingArtists.Add(albumArtist);
            }

            await _albumRepository.AddAsync(song.Album);
            existingAlbums.Add(song.Album);
            _logger.LogDebug("Added new album: {AlbumName}", song.Album.Name);
        }
    }

    private async Task ProcessGenresAsync(Song song, List<Genre> existingGenres)
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
                await _genreRepository.AddAsync(genre);
                existingGenres.Add(genre);
                song.Genres.Add(genre);
                _logger.LogDebug("Added new genre: {GenreName}", genre.Name);
            }
        }
    }
}