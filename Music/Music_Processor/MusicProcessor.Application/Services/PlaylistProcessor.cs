using System.Collections.Concurrent;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Services;

public class PlaylistProcessor : IPlaylistProcessor
{
    private readonly IFileService _fileService;
    private readonly MetadataHandlerFactory _metadataHandlerFactory;

    private readonly ISongRepository _songRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IStyleRepository _styleRepository;
    private readonly IArtistRepository _artistRepository;

    public PlaylistProcessor(
        IFileService fileService,
        ISongRepository songRepository,
        MetadataHandlerFactory metadataHandlerFactory,
        IArtistRepository artistRepository,
        IStyleRepository styleRepository,
        IGenreRepository genreRepository)
    {
        _fileService = fileService;
        _songRepository = songRepository;
        _metadataHandlerFactory = metadataHandlerFactory;
        _artistRepository = artistRepository;
        _styleRepository = styleRepository;
        _genreRepository = genreRepository;
    }

    public async Task WriteSongsToDbAsync(string playlistPath)
    {
        var playlistSongs = _fileService.GetAllAudioFilesInFolder(playlistPath);
        var existingTitles = _songRepository.GetAll().Select(s => s.Title).ToList();
        var songsToAdd = new List<Song>();

        // Load all existing entities
        var existingArtists = await _artistRepository.GetAllAsync();
        var existingGenres = await _genreRepository.GetAllAsync();
        var existingStyles = await _styleRepository.GetAllAsync();

        // Process each song
        foreach (var songFile in playlistSongs)
        {
            var handler = _metadataHandlerFactory.GetHandler(songFile);
            var metadata = handler.ExtractMetadata(songFile);

            if (existingTitles.Contains(metadata.Title))
            {
                continue;
            }

            var song = new Song(
                metadata.Title,
                metadata.Album,
                metadata.Year,
                metadata.Comment,
                metadata.TrackNumber,
                metadata.Duration,
                metadata.FileType
            );

            // Process each type of metadata
            existingTitles.Add(song.Title);

            song.Artists = ProcessArtists(metadata.Artists, existingArtists);
            existingArtists.AddRange(song.Artists);

            song.Genres = ProcessGenres(metadata.Genres, existingGenres);
            existingGenres.AddRange(song.Genres);

            song.Styles = ProcessStyles(metadata.Styles, existingStyles);
            existingStyles.AddRange(song.Styles);

            songsToAdd.Add(song);

            if (songsToAdd.Count >= 100)
            {
                await _songRepository.AddRangeAsync(songsToAdd);
                songsToAdd.Clear();
            }
        }

        if (songsToAdd.Any())
        {
            await _songRepository.AddRangeAsync(songsToAdd);
        }
    }

    private ICollection<Artist> ProcessArtists(IEnumerable<Artist> newArtists, List<Artist> existingArtists)
    {
        var processedArtists = new List<Artist>();

        foreach (var artist in newArtists)
        {
            var existingArtist = existingArtists.FirstOrDefault(a => string.Equals(a.Name, artist.Name, StringComparison.OrdinalIgnoreCase));

            if (existingArtist != null)
            {
                processedArtists.Add(existingArtist);
            }
            else
            {
                var newArtist = new Artist { Name = artist.Name };
                existingArtists.Add(newArtist);
                processedArtists.Add(newArtist);
            }
        }

        return processedArtists;
    }

    private ICollection<Genre> ProcessGenres(IEnumerable<Genre> newGenres, List<Genre> discoveredGenres)
    {
        var processedGenres = new List<Genre>();

        foreach (var genre in newGenres)
        {
            var existingGenre = discoveredGenres.FirstOrDefault(g =>
                string.Equals(g.Name, genre.Name, StringComparison.OrdinalIgnoreCase));

            if (existingGenre != null)
            {
                processedGenres.Add(existingGenre);
            }
            else
            {
                var newGenre = new Genre { Name = genre.Name };
                discoveredGenres.Add(newGenre);
                processedGenres.Add(newGenre);
            }
        }

        return processedGenres;
    }

    private ICollection<Style> ProcessStyles(IEnumerable<Style> newStyles, List<Style> discoveredStyles)
    {
        var processedStyles = new List<Style>();

        foreach (var style in newStyles)
        {
            var existingStyle = discoveredStyles.FirstOrDefault(s =>
                string.Equals(s.Name, style.Name, StringComparison.OrdinalIgnoreCase));

            if (existingStyle != null)
            {
                processedStyles.Add(existingStyle);
            }
            else
            {
                var newStyle = new Style { Name = style.Name };
                discoveredStyles.Add(newStyle);
                processedStyles.Add(newStyle);
            }
        }

        return processedStyles;
    }
}