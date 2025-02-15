using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.FixMetadata;

public sealed class FixMetadataHandler(
    IGenreRepository genreRepository,
    IGenreCategoryRepository genreCategoryRepository,
    ISongRepository songRepository,
    ILogger<FixMetadataHandler> logger)
    : IRequestHandler<FixMetadataCommand>
{
    public async Task Handle(FixMetadataCommand request, CancellationToken cancellationToken)
    {
        // Fetch all data upfront
        var songs = (await songRepository.GetAllAsync()).ToList();
        var existingGenres = (await genreCategoryRepository.GetAllAsync()).ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);
        var existingStyles = (await genreRepository.GetAllAsync()).ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("Processing {SongCount} songs...", songs.Count);

        var modifiedSongs = new List<Song>();

        foreach (var song in songs)
        {
            var stylesToRemove = new HashSet<Genre>();

            // Process styles and genres for the song
            foreach (var style in song.Genres.ToList())
            {
                if (!existingStyles.TryGetValue(style.Name, out var existingStyle))
                {
                    logger.LogWarning("Genre not found in repository: {genreName}", style.Name);
                    continue;
                }

                // Mark style for removal if flagged or matches a genre name
                if (existingStyle.RemoveFromSongs || existingGenres.ContainsKey(existingStyle.Name))
                {
                    stylesToRemove.Add(style);
                    logger.LogInformation("Marking style for removal: {genreName} ({Reason})", style.Name, existingStyle.RemoveFromSongs ? "flagged for removal" : "matches genre name");
                }
            }

            // Remove styles and add genres
            if (stylesToRemove.Count > 0)
            {
                // Remove styles marked for removal
                foreach (var style in stylesToRemove) song.Genres.Remove(style);

                modifiedSongs.Add(song);
                logger.LogInformation("Song modified: {SongTitle}", song.Title);
            }
            else
            {
                logger.LogInformation("No modifications needed for song: {SongTitle}", song.Title);
            }
        }

        // Perform bulk updates for modified songs
        if (modifiedSongs.Count > 0)
        {
            await songRepository.UpdateRangeAsync(modifiedSongs);
            logger.LogInformation("Updated {ModifiedSongCount} songs.", modifiedSongs.Count);
        }
        else
        {
            logger.LogInformation("No songs were modified.");
        }
    }
}