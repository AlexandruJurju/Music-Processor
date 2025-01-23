using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Models.Config;

namespace MusicProcessor.Infrastructure.Config;

public class ConfigRepository(
    ILogger<ConfigRepository> logger,
    IFileService fileService,
    IStyleRepository styleRepository
) : IConfigRepository
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task WriteStyleMappingAsync()
    {
        var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "genre-mappings.json");

        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
            logger.LogInformation("{Message}", $"Deleted existing mapping file at {jsonPath}");
        }

        var styles = await styleRepository.GetAllAsync();
        logger.LogInformation("{Message}", $"Loaded {styles.Count} styles");

        var styleMappings = styles.Select(style => new StyleMappingDTO(
            style.Name,
            style.RemoveFromSongs,
            style.Genres.Select(g => g.Name).ToList()
        )).ToList();

        var jsonString = JsonSerializer.Serialize(styleMappings, _jsonOptions);
        await File.WriteAllTextAsync(jsonPath, jsonString);
        logger.LogInformation("{Message}", $"Written the mapping file to {jsonPath}");
    }

    public async Task<IEnumerable<Style>> ReadStyleMappingAsync()
    {
        var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "genre-mappings.json");

        var styleDTOs = await JsonSerializer.DeserializeAsync<List<StyleMappingDTO>>(File.OpenRead(jsonPath), _jsonOptions);

        if (styleDTOs is not null)
            return styleDTOs.Select(dto => new Style(dto.StyleName)
            {
                RemoveFromSongs = dto.RemoveFromSongs,
                Genres = dto.GenreNames.Select(name => new Genre(name)).ToList()
            });
        logger.LogWarning("{Message}", "Style mapping file not found");
        throw new FileNotFoundException("Style mapping file not found");
    }
}