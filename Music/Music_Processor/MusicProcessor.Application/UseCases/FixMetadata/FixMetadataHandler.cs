using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.FixMetadata;

public sealed class FixMetadataHandler(
    IStyleRepository styleRepository,
    IGenreRepository genreRepository,
    ISongRepository songRepository,
    ILogger<FixMetadataHandler> logger)
    : IRequestHandler<FixMetadataCommand>
{
    public async Task Handle(FixMetadataCommand request, CancellationToken cancellationToken)
    {
        // Get all songs, genres and styles from repositories
        var songs = await songRepository.GetAllAsync();
        var existingGenreNames = (await genreRepository.GetAllAsync())
            .ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);
        var existingStyleNames = (await styleRepository.GetAllAsync())
            .ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("{Message}", $"Processing {songs.Count} songs...");

        foreach (var song in songs)
        {
            logger.LogInformation("{Message}", $"Processing song: {song.Title}");
            logger.LogInformation("{Message}", $"Current styles: {string.Join(", ", song.Styles.Select(s => s.Name))}");
            logger.LogInformation("{Message}", $"Current genres: {string.Join(", ", song.Genres.Select(g => g.Name))}");

            // Track styles to remove and genres to add for this song
            var stylesToRemove = new HashSet<Style>();
            var genresToAdd = new HashSet<Genre>();

            // Look over all the styles in the song
            foreach (var style in song.Styles)
            {
                // Get the reference style from repository (if it exists)
                if (!existingStyleNames.TryGetValue(style.Name, out var existingStyle))
                {
                    logger.LogWarning("{Message}", $"Style not found in repository: {style.Name}");
                    continue;
                }

                // Process genres from the style
                foreach (var styleGenre in existingStyle.Genres)
                    if (existingGenreNames.TryGetValue(styleGenre.Name, out var matchingGenre))
                    {
                        genresToAdd.Add(matchingGenre);
                        logger.LogInformation("{Message}", $"Adding genre from style {style.Name}: {matchingGenre.Name}");
                    }

                // Mark style for removal if it's flagged for removal or matches a genre name
                if (existingStyle.RemoveFromSongs || existingGenreNames.ContainsKey(existingStyle.Name))
                {
                    stylesToRemove.Add(style);
                    var reason = existingStyle.RemoveFromSongs ? "(flagged for removal)" : "(matches genre name)";
                    logger.LogInformation("{Message}", $"Marking style for removal: {style.Name} {reason}");

                    // If removing because style name matches a genre, add that genre
                    if (existingGenreNames.TryGetValue(style.Name, out var matchingGenre))
                    {
                        genresToAdd.Add(matchingGenre);
                        logger.LogInformation("{Message}", $"Will add matching genre: {matchingGenre.Name}");
                    }
                }
            }

            var songModified = false;

            // Remove marked styles from the song
            foreach (var style in stylesToRemove)
            {
                song.Styles.Remove(style);
                songModified = true;
            }

            // Add new genres to the song, avoiding duplicates
            foreach (var genre in genresToAdd)
                if (!song.Genres.Any(g => string.Equals(g.Name, genre.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    song.Genres.Add(genre);
                    songModified = true;
                }

            // Update the song in repository if changes were made
            if (songModified)
            {
                // todo: readd update
                await songRepository.UpdateAsync(song);
                logger.LogInformation("{Message}", $"Song modified: {song.Title}");
                logger.LogInformation("{Message}", $"Final styles: {string.Join(", ", song.Styles.Select(s => s.Name))}");
                logger.LogInformation("{Message}", $"Final genres: {string.Join(", ", song.Genres.Select(g => g.Name))}");
            }
            else
            {
                logger.LogInformation("{Message}", "No modifications needed.");
            }
        }
    }
}