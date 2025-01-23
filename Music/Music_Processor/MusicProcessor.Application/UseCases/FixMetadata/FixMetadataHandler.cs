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
        // Fetch all data upfront
        var songs = (await songRepository.GetAllAsync()).ToList();
        var existingGenres = (await genreRepository.GetAllAsync()).ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);
        var existingStyles = (await styleRepository.GetAllAsync()).ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("Processing {SongCount} songs...", songs.Count);

        var modifiedSongs = new List<Song>();

        foreach (var song in songs)
        {
            var stylesToRemove = new HashSet<Style>();

            // Process styles and genres for the song
            foreach (var style in song.Styles.ToList())
            {
                if (!existingStyles.TryGetValue(style.Name, out var existingStyle))
                {
                    logger.LogWarning("Style not found in repository: {StyleName}", style.Name);
                    continue;
                }

                // Mark style for removal if flagged or matches a genre name
                if (existingStyle.RemoveFromSongs || existingGenres.ContainsKey(existingStyle.Name))
                {
                    stylesToRemove.Add(style);
                    logger.LogInformation("Marking style for removal: {StyleName} ({Reason})", style.Name, existingStyle.RemoveFromSongs ? "flagged for removal" : "matches genre name");
                }
            }

            // Remove styles and add genres
            if (stylesToRemove.Count > 0)
            {
                // Remove styles marked for removal
                foreach (var style in stylesToRemove)
                {
                    song.Styles.Remove(style);
                }

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