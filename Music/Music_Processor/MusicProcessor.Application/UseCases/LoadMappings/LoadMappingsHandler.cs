using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.LoadMappings;

public class LoadMappingsHandler(
    IConfigRepository configRepository,
    IStyleRepository styleRepository,
    IGenreRepository genreRepository,
    ILogger<LoadMappingsHandler> logger
)
    : IRequestHandler<LoadMappingsCommand>
{
    public async Task Handle(LoadMappingsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Message}", $"Starting style mappings load");
        var mappedStyles = (await configRepository.ReadStyleMappingAsync()).ToList();
        logger.LogInformation("{Message}", $"Found {mappedStyles.Count} style mappings to process");

        foreach (var style in mappedStyles)
        {
            try
            {
                logger.LogInformation("{Message}", $"Processing style: '{style.Name}'");
                var existingStyle = await styleRepository.GetByNameAsync(style.Name);
                var isNewStyle = existingStyle is null;

                if (isNewStyle)
                {
                    logger.LogInformation("{Message}", $"Creating new style: '{style.Name}'");
                    existingStyle = new Style(style.Name, style.RemoveFromSongs);
                }
                else
                {
                    logger.LogInformation("{Message}", $"Updating existing style: '{style.Name}'");
                    existingStyle!.Genres.Clear();
                    existingStyle.RemoveFromSongs = style.RemoveFromSongs;
                    await styleRepository.UpdateAsync(existingStyle);
                }

                foreach (var genre in style.Genres)
                {
                    var existingGenre = await genreRepository.GetByNameAsync(genre.Name);
                    if (existingGenre == null)
                    {
                        logger.LogInformation("{Message}", $"Creating new genre: '{genre.Name}'");
                        existingGenre = await genreRepository.AddAsync(new Genre(genre.Name));
                    }

                    existingStyle.Genres.Add(existingGenre);
                }

                if (isNewStyle)
                {
                    await styleRepository.AddAsync(existingStyle);
                    logger.LogInformation("{Message}", $"Added new style: '{style.Name}' with {existingStyle.Genres.Count} genres");
                }
                else
                {
                    await styleRepository.UpdateAsync(existingStyle);
                    logger.LogInformation("{Message}", $"Updated style: '{style.Name}' with {existingStyle.Genres.Count} genres");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", $"Failed to process style: '{style.Name}'");
                throw;
            }
        }

        logger.LogInformation("{Message}", $"Completed style mappings load");
    }
}