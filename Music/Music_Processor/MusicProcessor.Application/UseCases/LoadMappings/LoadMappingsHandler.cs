using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Entities.GenreCategories;
using MusicProcessor.Domain.Entities.Genres;

namespace MusicProcessor.Application.UseCases.LoadMappings;

public class LoadMappingsHandler : IRequestHandler<LoadMappingsCommand>
{
    private readonly IGenreSyncService _genreSyncService;
    private readonly IGenreRepository _genreRepository;
    private readonly IGenreCategoryRepository _genreCategoryRepository;
    private readonly ILogger<LoadMappingsHandler> _logger;

    public LoadMappingsHandler(IGenreSyncService genreSyncService,
        IGenreRepository genreRepository,
        IGenreCategoryRepository genreCategoryRepository,
        ILogger<LoadMappingsHandler> logger)
    {
        _genreSyncService = genreSyncService;
        _genreRepository = genreRepository;
        _genreCategoryRepository = genreCategoryRepository;
        _logger = logger;
    }

    public async Task Handle(LoadMappingsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Message}", "Starting style mappings load");

        // Fetch all style mappings and existing styles/genres
        var mappedGenres = (await _genreSyncService.ReadStyleMappingAsync()).ToList();
        var existingGenres = (await _genreRepository.GetAllAsync()).ToDictionary(s => s.Name);
        var existingGenreCategories = (await _genreCategoryRepository.GetAllAsync()).ToDictionary(g => g.Name);

        _logger.LogInformation("{Message}", $"Found {mappedGenres.Count} genre mappings to process");

        var genresToAdd = new List<Genre>();
        var genresToUpdate = new List<Genre>();
        var genreCategoriesToAdd = new List<GenreCategory>();

        // Process each style in the mapped configuration
        foreach (var genre in mappedGenres)
        {
            _logger.LogInformation("{Message}", $"Processing genre: '{genre.Name}'");

            // Check if the style already exists
            if (!existingGenres.TryGetValue(genre.Name, out var existingStyle))
            {
                // Create a new style if it doesn't exist
                _logger.LogInformation("{Message}", $"Creating new genre: '{genre.Name}'");
                existingStyle = new Genre(genre.Name, genre.RemoveFromSongs);
                genresToAdd.Add(existingStyle);
            }
            else
            {
                // Update the existing style if necessary
                if (existingStyle.RemoveFromSongs != genre.RemoveFromSongs)
                {
                    _logger.LogInformation("{Message}", $"Updating existing genre: '{genre.Name}'");
                    existingStyle.RemoveFromSongs = genre.RemoveFromSongs;
                    genresToUpdate.Add(existingStyle);
                }
            }

            // // Process genres for the current style
            // var genresToRemove = existingStyle.GenreCategories
            //     .Where(g => !style.GenreCategories.Any(mappedGenre => mappedGenre.Name == g.Name))
            //     .ToList();
            //
            // // Remove genres that are no longer in the mapped configuration
            // foreach (var genre in genresToRemove)
            // {
            //     existingStyle.GenreCategories.Remove(genre);
            //     logger.LogInformation("{Message}", $"Removed genre '{genre.Name}' from style '{style.Name}'");
            // }

            // remove existing genres from the style
            existingStyle.GenreCategories.Clear();

            // Add or update genres for the current style
            foreach (var genreCategory in genre.GenreCategories)
            {
                if (!existingGenreCategories.TryGetValue(genreCategory.Name, out var existingGenre))
                {
                    // Create a new genre if it doesn't exist
                    _logger.LogInformation("{Message}", $"Creating new genre category: '{genreCategory.Name}'");
                    existingGenre = new GenreCategory(genreCategory.Name);
                    genreCategoriesToAdd.Add(existingGenre);
                    // Add to lookup for subsequent iterations
                    existingGenreCategories[genreCategory.Name] = existingGenre;
                }

                // Add the genre to the style if not already present
                if (!existingStyle.GenreCategories.Any(g => g.Name == genreCategory.Name))
                {
                    existingStyle.GenreCategories.Add(existingGenre);
                    _logger.LogInformation("{Message}", $"Added genre category '{genreCategory.Name}' to style '{genreCategory.Name}'");
                }
            }
        }

        // Perform bulk operations for genres and styles
        if (genreCategoriesToAdd.Any())
        {
            await _genreCategoryRepository.AddRangeAsync(genreCategoriesToAdd);
            _logger.LogInformation("{Message}", $"Added {genreCategoriesToAdd.Count} new genres");
        }

        if (genresToAdd.Any())
        {
            await _genreRepository.AddRangeAsync(genresToAdd);
            _logger.LogInformation("{Message}", $"Added {genresToAdd.Count} new styles");
        }

        if (genresToUpdate.Any())
        {
            await _genreRepository.UpdateRangeAsync(genresToUpdate);
            _logger.LogInformation("{Message}", $"Updated {genresToUpdate.Count} styles");
        }

        _logger.LogInformation("{Message}", "Completed style mappings load");
    }
}