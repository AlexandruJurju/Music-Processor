using ATL;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Entities.Albums;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.Songs;
using Constants = MusicProcessor.Domain.Constants.Constants;

namespace MusicProcessor.Application.Services;

public class MetadataService : IMetadataService
{
    private readonly ILogger<MetadataService> _logger;

    public MetadataService(ILogger<MetadataService> logger)
    {
        _logger = logger;
    }

    public void WriteMetadata(Song song)
    {
        try
        {
            var track = new Track(song.FilePath);
            UpdateMetadata(track, song);
            track.Save();
            _logger.LogDebug("Successfully updated metadata for file: {FilePath}", song.FilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata for file: {FilePath}", song.FilePath);
            throw new Exception($"Error updating metadata for file {song.FilePath}: {ex.Message}", ex);
        }
    }

    public Song ReadMetadata(string songPath)
    {
        try
        {
            var track = new Track(songPath);
            var metadata = ExtractMetadata(track, songPath);
            _logger.LogDebug("Successfully read metadata from file: {FilePath}", songPath);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading metadata from file: {FilePath}", songPath);
            throw new Exception($"Error reading metadata from file {songPath}: {ex.Message}", ex);
        }
    }

    private void UpdateMetadata(Track track, Song song)
    {
        _logger.LogDebug("Updating metadata for file: {FilePath}", song.FilePath);
        track.Title = song.Name;
        track.Album = song.Album?.Name;
        track.Year = song.Date;
        UpdateAdditionalMetadata(track, song);
    }

    private void UpdateAdditionalMetadata(Track track, Song song)
    {
        WriteGenreCategories(track, song);

        WriteGenres(track, song);
    }

    private void WriteGenres(Track track, Song song)
    {
        var genres = song.Genres
            .Where(g => !g.RemoveFromSongs)
            .Select(g => g.Name)
            .ToArray();

        if (genres.Any())
        {
            _logger.LogDebug("Setting {Count} genres: {Genres}", genres.Length, string.Join(", ", genres));
            track.Genre = string.Join(";", genres);
        }
        else
        {
            _logger.LogDebug("No genres to set for file: {FilePath}", song.FilePath);
        }
    }

    private void WriteGenreCategories(Track track, Song song)
    {
        _logger.LogDebug("Updating additional metadata for file: {FilePath}", song.FilePath);
        var genreCategories = song.Genres
            .SelectMany(g => g.GenreCategories.Select(c => c.Name))
            .Distinct()
            .ToArray();

        if (genreCategories.Any())
        {
            _logger.LogDebug("Setting {Count} genre categories: {GenreCategories}", genreCategories.Length, string.Join(", ", genreCategories));
            track.AdditionalFields[Constants.GENRE_CATEGORY] = string.Join(";", genreCategories);
        }
    }

    private Song ExtractMetadata(Track track, string songPath)
    {
        var genres = ExtractGenres(track).Select(genreName => new Genre { Name = genreName }).ToList();

        // Log genres extracted
        _logger.LogDebug("Extracted {Count} genres from file: {FilePath}", genres.Count, songPath);

        var metadata = new Song(
            name: track.Title,
            isrc: track.ISRC,
            artists: ExtractArtists(track),
            mainArtist: new Artist(track.Artist),
            genres: genres,
            discNumber: track.DiscNumber ?? 0,
            discCount: track.DiscTotal ?? 0,
            album: track.Album != null ? new Album(track.Album, new Artist(track.AlbumArtist)) : null,
            duration: track.Duration,
            date: track.Year ?? 0,
            trackNumber: track.TrackNumber ?? 0,
            tracksCount: track.TrackTotal ?? 0
        )
        {
            FilePath = songPath,
            FileType = Path.GetExtension(songPath).ToLowerInvariant()
        };

        return metadata;
    }

    private List<string> ExtractGenres(Track track)
    {
        _logger.LogDebug("Extracting genres from track");
        var genres = new List<string>();

        genres.AddRange(track.Genre
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g)));

        return genres.Distinct().ToList();
    }

    private ICollection<Artist> ExtractArtists(Track track)
    {
        _logger.LogDebug("Extracting artists from track");

        var artists = new List<Artist>();

        return artists;
    }
}