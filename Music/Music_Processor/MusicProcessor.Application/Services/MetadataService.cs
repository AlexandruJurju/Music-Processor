using ATL;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Entities;
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

    private Song ExtractMetadata(Track track, string songPath)
    {
        var metadata = new Song(
            songPath,
            track.Title ?? Path.GetFileNameWithoutExtension(songPath),
            track.Album != null ? new Album(track.Album) : null,
            track.Year,
            track.Comment,
            track.TrackNumber ?? 0,
            TimeSpan.FromMilliseconds(track.DurationMs),
            Path.GetExtension(songPath).ToLowerInvariant()
        );

        metadata.MainArtist = new Artist(track.Artist);

        foreach (var performer in track.Artist.Split('/'))
        {
            var performerName = performer.Trim();
            if (!string.IsNullOrWhiteSpace(performerName))
            {
                metadata.Artists.Add(new Artist { Name = performerName });
            }
        }

        var genres = ExtractGenres(track);
        foreach (var genreName in genres)
        {
            metadata.Genres.Add(new Genre { Name = genreName });
        }

        _logger.LogDebug("Extracted {Count} genres from file: {FilePath}", genres.Count, songPath);
        return metadata;
    }

    private void UpdateMetadata(Track track, Song song)
    {
        _logger.LogDebug("Updating metadata for file: {FilePath}", song.FilePath);
        track.Title = song.Title;
        track.Album = song.Album?.Name;
        track.Year = song.Year;
        track.Comment = song.Comment;
        UpdateAdditionalMetadata(track, song);
    }

    private void UpdateAdditionalMetadata(Track track, Song song)
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

    private List<string> ExtractGenres(Track track)
    {
        _logger.LogDebug("Extracting genres from file");
        var genres = new List<string>();

        genres.AddRange(track.Genre
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g)));

        return genres.Distinct().ToList();
    }
}