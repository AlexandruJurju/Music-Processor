using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.LoadMappings;

public class LoadMappingsHandler(
    IStyleConfigRepository styleConfigRepository,
    IStyleRepository styleRepository,
    IGenreRepository genreRepository,
    ILogger<LoadMappingsHandler> logger
) : IRequestHandler<LoadMappingsCommand>
{
    public async Task Handle(LoadMappingsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Message}", "Starting style mappings load");

        // Fetch all style mappings and existing styles/genres
        var mappedStyles = (await styleConfigRepository.ReadStyleMappingAsync()).ToList();
        var existingStyles = (await styleRepository.GetAllAsync()).ToDictionary(s => s.Name);
        var existingGenres = (await genreRepository.GetAllAsync()).ToDictionary(g => g.Name);

        logger.LogInformation("{Message}", $"Found {mappedStyles.Count} style mappings to process");

        var stylesToAdd = new List<Style>();
        var stylesToUpdate = new List<Style>();
        var genresToAdd = new List<Genre>();

        // Process each style in the mapped configuration
        foreach (var style in mappedStyles)
        {
            logger.LogInformation("{Message}", $"Processing style: '{style.Name}'");

            // Check if the style already exists
            if (!existingStyles.TryGetValue(style.Name, out var existingStyle))
            {
                // Create a new style if it doesn't exist
                logger.LogInformation("{Message}", $"Creating new style: '{style.Name}'");
                existingStyle = new Style(style.Name, style.RemoveFromSongs);
                stylesToAdd.Add(existingStyle);
            }
            else
            {
                // Update the existing style if necessary
                if (existingStyle.RemoveFromSongs != style.RemoveFromSongs)
                {
                    logger.LogInformation("{Message}", $"Updating existing style: '{style.Name}'");
                    existingStyle.RemoveFromSongs = style.RemoveFromSongs;
                    stylesToUpdate.Add(existingStyle);
                }
            }

            // // Process genres for the current style
            // var genresToRemove = existingStyle.Genres
            //     .Where(g => !style.Genres.Any(mappedGenre => mappedGenre.Name == g.Name))
            //     .ToList();
            //
            // // Remove genres that are no longer in the mapped configuration
            // foreach (var genre in genresToRemove)
            // {
            //     existingStyle.Genres.Remove(genre);
            //     logger.LogInformation("{Message}", $"Removed genre '{genre.Name}' from style '{style.Name}'");
            // }

            // remove existing genres from the style
            existingStyle.Genres.Clear();

            // Add or update genres for the current style
            foreach (var genre in style.Genres)
            {
                if (!existingGenres.TryGetValue(genre.Name, out var existingGenre))
                {
                    // Create a new genre if it doesn't exist
                    logger.LogInformation("{Message}", $"Creating new genre: '{genre.Name}'");
                    existingGenre = new Genre(genre.Name);
                    genresToAdd.Add(existingGenre);
                    // Add to lookup for subsequent iterations
                    existingGenres[genre.Name] = existingGenre; 
                }

                // Add the genre to the style if not already present
                if (!existingStyle.Genres.Any(g => g.Name == genre.Name))
                {
                    existingStyle.Genres.Add(existingGenre);
                    logger.LogInformation("{Message}", $"Added genre '{genre.Name}' to style '{style.Name}'");
                }
            }
        }

        // Perform bulk operations for genres and styles
        if (genresToAdd.Any())
        {
            await genreRepository.AddRangeAsync(genresToAdd);
            logger.LogInformation("{Message}", $"Added {genresToAdd.Count} new genres");
        }

        if (stylesToAdd.Any())
        {
            await styleRepository.AddRangeAsync(stylesToAdd);
            logger.LogInformation("{Message}", $"Added {stylesToAdd.Count} new styles");
        }

        if (stylesToUpdate.Any())
        {
            await styleRepository.UpdateRangeAsync(stylesToUpdate);
            logger.LogInformation("{Message}", $"Updated {stylesToUpdate.Count} styles");
        }

        logger.LogInformation("{Message}", "Completed style mappings load");
    }
}