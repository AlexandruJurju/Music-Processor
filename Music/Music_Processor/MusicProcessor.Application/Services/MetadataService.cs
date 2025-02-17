using ATL;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Entities;
using Constants = MusicProcessor.Domain.Constants.Constants;

namespace MusicProcessor.Application.Services;

public class MetadataService(ILogger<MetadataService> logger) : IMetadataService
{
    public void WriteMetadata(Song song)
    {
        try
        {
            var track = new Track(song.FilePath);
            UpdateMetadata(track, song);
            track.Save();
            logger.LogInformation("Successfully updated metadata for file: {FilePath}", song.FilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating metadata for file: {FilePath}", song.FilePath);
            throw;
        }
    }

    public Song ReadMetadata(string songFile)
    {
        try
        {
            var track = new Track(songFile);
            return ExtractMetadata(track, songFile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reading metadata from file: {FilePath}", songFile);
            throw;
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

        foreach (var performer in track.Artist.Split('/'))
        {
            var performerName = performer.Trim();
            if (!string.IsNullOrWhiteSpace(performerName))
            {
                metadata.Artists.Add(new Artist { Name = performerName });
            }
        }

        foreach (var genreName in ExtractGenres(track))
        {
            metadata.Genres.Add(new Genre { Name = genreName });
        }

        return metadata;
    }

    private void UpdateMetadata(Track track, Song song)
    {
        track.Title = song.Title;
        track.Album = song.Album?.Name;
        track.Year = song.Year;
        track.Comment = song.Comment;

        track.Artist = string.Join("/", song.Artists.Select(a => a.Name));

        track.Genre = string.Join(";", song.Genres.Select(g => g.Name));
        
        UpdateAdditionalMetadata(track, song);
    }

    private void UpdateAdditionalMetadata(Track track, Song song)
    {
        var genreCategories = song.Genres
            .SelectMany(g => g.GenreCategories.Select(c => c.Name))
            .Distinct()
            .ToArray();

        if (genreCategories.Any())
        {
            track.AdditionalFields[Constants.GENRE_CATEGORY] = string.Join(";", genreCategories);
        }

        var genres = song.Genres
            .Where(g => !g.RemoveFromSongs)
            .Select(g => g.Name)
            .ToArray();

        track.Genre = string.Join(";", genres);
    }

    private List<string> ExtractGenres(Track track)
    {
        var genres = new List<string>();

        genres.AddRange(track.Genre
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g)));

        return genres.Distinct().ToList();
    }
}