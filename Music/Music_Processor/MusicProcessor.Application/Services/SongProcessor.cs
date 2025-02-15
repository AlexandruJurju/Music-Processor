using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Services;

public class SongProcessor(
    IGenreCategoryRepository genreCategoryRepository,
    IArtistRepository artistRepository,
    IGenreRepository genreRepository,
    ISongRepository songRepository,
    IAlbumRepository albumRepository,
    ILogger<SongProcessor> logger) : ISongProcessor
{
    private Dictionary<string, Album> _existingAlbums = new();
    private Dictionary<string, Artist> _existingArtists = new();
    private Dictionary<string, GenreCategory> _existingGenres = new();
    private Dictionary<string, Genre> _existingStyles = new();

    public async Task AddRawSongsToDbAsync(IEnumerable<Song> songs)
    {
        var songsList = songs.ToList();
        logger.LogInformation($"Processing {songsList.Count} songs");

        // Load all existing entities
        await LoadEntities();

        foreach (var song in songsList)
        {
            logger.LogInformation($"Processing song: {song.Title} by {string.Join(", ", song.Artists.Select(a => a.Name))}");
            ProcessSongEntities(song);
        }

        await songRepository.AddRangeAsync(songsList);
        logger.LogInformation($"Successfully added {songsList.Count} songs to database");
    }

    private async Task LoadEntities()
    {
        _existingArtists = (await artistRepository.GetAllAsync()).ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        logger.LogInformation($"Loaded {_existingArtists.Count} existing artists");

        _existingGenres = (await genreCategoryRepository.GetAllAsync()).ToDictionary(g => g.Name, StringComparer.OrdinalIgnoreCase);
        logger.LogInformation($"Loaded {_existingGenres.Count} existing genres");

        _existingStyles = (await genreRepository.GetAllAsync()).ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        logger.LogInformation($"Loaded {_existingStyles.Count} existing styles");

        _existingAlbums = (await albumRepository.GetAllAlbumsAsync()).ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        logger.LogInformation($"Loaded {_existingAlbums.Count} existing albums");
    }

    private void ProcessSongEntities(Song song)
    {
        ProcessAlbums(song);
        ProcessArtists(song);
        ProcessGenres(song);
    }

    private void ProcessAlbums(Song song)
    {
        if (song.Album is null) return;

        if (_existingAlbums.TryGetValue(song.Album.Name, out var album))
        {
            song.Album = album;
            logger.LogDebug($"Using existing album: {album.Name}");
        }
        else
        {
            _existingAlbums.Add(song.Album.Name, song.Album);
            logger.LogDebug($"Adding new album: {song.Album.Name}");
        }
    }

    private void ProcessGenres(Song song)
    {
        var genreList = song.Genres.ToList();
        song.Genres.Clear();
        foreach (var style in genreList)
            if (_existingStyles.TryGetValue(style.Name, out var existingGenre))
            {
                song.Genres.Add(existingGenre);
                logger.LogDebug($"Using existing style: {style.Name}");
            }
            else
            {
                _existingStyles[style.Name] = style;
                song.Genres.Add(style);
                logger.LogDebug($"Adding new style: {style.Name}");
            }
    }

    private void ProcessArtists(Song song)
    {
        var artistsList = song.Artists.ToList();
        song.Artists.Clear();
        foreach (var artist in artistsList)
            if (_existingArtists.TryGetValue(artist.Name, out var existingArtist))
            {
                song.Artists.Add(existingArtist);
                logger.LogDebug($"Using existing artist: {artist.Name}");
            }
            else
            {
                _existingArtists[artist.Name] = artist;
                song.Artists.Add(artist);
                logger.LogDebug($"Adding new artist: {artist.Name}");
            }
    }
}