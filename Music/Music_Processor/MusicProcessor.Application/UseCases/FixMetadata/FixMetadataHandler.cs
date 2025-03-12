using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Entities.Songs;

namespace MusicProcessor.Application.UseCases.FixMetadata;

public sealed class FixMetadataHandler : IRequestHandler<FixMetadataCommand>
{
    private readonly IGenreCategoryRepository _genreCategoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ILogger<FixMetadataHandler> _logger;
    private readonly ISongRepository _songRepository;

    public FixMetadataHandler(IGenreRepository genreRepository,
        IGenreCategoryRepository genreCategoryRepository,
        ISongRepository songRepository,
        ILogger<FixMetadataHandler> logger)
    {
        _genreRepository = genreRepository;
        _genreCategoryRepository = genreCategoryRepository;
        _songRepository = songRepository;
        _logger = logger;
    }

    public async Task Handle(FixMetadataCommand request, CancellationToken cancellationToken)
    {
        // Fetch all data upfront
        var songs = (await _songRepository.GetAllAsync()).ToList();
        var existingGenres = (await _genreCategoryRepository.GetAllAsync()).ToDictionary(g => g.Name, g => g, StringComparer.OrdinalIgnoreCase);
        var existingStyles = (await _genreRepository.GetAllAsync()).ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

        _logger.LogInformation("Processing {SongCount} songs...", songs.Count);

        var modifiedSongs = new List<Song>();

        foreach (var song in songs)
        {
            var stylesToRemove = new HashSet<Genre>();

            // Process styles and genres for the song
            foreach (var style in song.Genres.ToList())
            {
                if (!existingStyles.TryGetValue(style.Name, out var existingStyle))
                {
                    _logger.LogWarning("Genre not found in repository: {genreName}", style.Name);
                    continue;
                }

                // Mark style for removal if flagged or matches a genre name
                if (existingStyle.RemoveFromSongs || existingGenres.ContainsKey(existingStyle.Name))
                {
                    stylesToRemove.Add(style);
                    _logger.LogInformation("Marking style for removal: {genreName} ({Reason})", style.Name, existingStyle.RemoveFromSongs ? "flagged for removal" : "matches genre name");
                }
            }

            // Remove styles and add genres
            if (stylesToRemove.Count > 0)
            {
                // Remove styles marked for removal
                foreach (var style in stylesToRemove) song.Genres.Remove(style);

                modifiedSongs.Add(song);
                _logger.LogInformation("Song modified: {SongTitle}", song.Name);
            }
            else
            {
                _logger.LogInformation("No modifications needed for song: {SongTitle}", song.Name);
            }
        }

        // Perform bulk updates for modified songs
        if (modifiedSongs.Count > 0)
        {
            await _songRepository.UpdateRangeAsync(modifiedSongs);
            _logger.LogInformation("Updated {ModifiedSongCount} songs.", modifiedSongs.Count);
        }
        else
        {
            _logger.LogInformation("No songs were modified.");
        }
    }
}