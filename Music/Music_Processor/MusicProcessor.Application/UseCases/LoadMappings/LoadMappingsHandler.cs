using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.LoadMappings;

public class LoadMappingsHandler(
    IGenreSyncService genreSyncService,
    IGenreRepository genreRepository,
    IGenreCategoryRepository genreCategoryRepository,
    ILogger<LoadMappingsHandler> logger
) : IRequestHandler<LoadMappingsCommand>
{
    public async Task Handle(LoadMappingsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Message}", "Starting style mappings load");

        // Fetch all style mappings and existing styles/genres
        var mappedGenres = (await genreSyncService.ReadStyleMappingAsync()).ToList();
        var existingGenres = (await genreRepository.GetAllAsync()).ToDictionary(s => s.Name);
        var existingGenreCategories = (await genreCategoryRepository.GetAllAsync()).ToDictionary(g => g.Name);

        logger.LogInformation("{Message}", $"Found {mappedGenres.Count} genre mappings to process");

        var genresToAdd = new List<Genre>();
        var genresToUpdate = new List<Genre>();
        var genreCategoriesToAdd = new List<GenreCategory>();

        // Process each style in the mapped configuration
        foreach (var genre in mappedGenres)
        {
            logger.LogInformation("{Message}", $"Processing genre: '{genre.Name}'");

            // Check if the style already exists
            if (!existingGenres.TryGetValue(genre.Name, out var existingStyle))
            {
                // Create a new style if it doesn't exist
                logger.LogInformation("{Message}", $"Creating new genre: '{genre.Name}'");
                existingStyle = new Genre(genre.Name, genre.RemoveFromSongs);
                genresToAdd.Add(existingStyle);
            }
            else
            {
                // Update the existing style if necessary
                if (existingStyle.RemoveFromSongs != genre.RemoveFromSongs)
                {
                    logger.LogInformation("{Message}", $"Updating existing genre: '{genre.Name}'");
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
                    logger.LogInformation("{Message}", $"Creating new genre category: '{genreCategory.Name}'");
                    existingGenre = new GenreCategory(genreCategory.Name);
                    genreCategoriesToAdd.Add(existingGenre);
                    // Add to lookup for subsequent iterations
                    existingGenreCategories[genreCategory.Name] = existingGenre;
                }

                // Add the genre to the style if not already present
                if (!existingStyle.GenreCategories.Any(g => g.Name == genreCategory.Name))
                {
                    existingStyle.GenreCategories.Add(existingGenre);
                    logger.LogInformation("{Message}", $"Added genre category '{genreCategory.Name}' to style '{genreCategory.Name}'");
                }
            }
        }

        // Perform bulk operations for genres and styles
        if (genreCategoriesToAdd.Any())
        {
            await genreCategoryRepository.AddRangeAsync(genreCategoriesToAdd);
            logger.LogInformation("{Message}", $"Added {genreCategoriesToAdd.Count} new genres");
        }

        if (genresToAdd.Any())
        {
            await genreRepository.AddRangeAsync(genresToAdd);
            logger.LogInformation("{Message}", $"Added {genresToAdd.Count} new styles");
        }

        if (genresToUpdate.Any())
        {
            await genreRepository.UpdateRangeAsync(genresToUpdate);
            logger.LogInformation("{Message}", $"Updated {genresToUpdate.Count} styles");
        }

        logger.LogInformation("{Message}", "Completed style mappings load");
    }
}