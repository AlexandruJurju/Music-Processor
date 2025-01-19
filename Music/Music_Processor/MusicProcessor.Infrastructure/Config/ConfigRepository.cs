using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

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
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task WriteStyleMappingAsync()
    {
        var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "genre-mappings.json");
        var styles = await styleRepository.GetAllAsync();
        logger.LogInformation("{Message}", $"Loaded {styles.Count} styles");

        var jsonString = JsonSerializer.Serialize(styles, _jsonOptions);
        await File.WriteAllTextAsync(jsonPath, jsonString);
        logger.LogInformation("{Message}", $"Written the mapping file to {jsonPath}");
    }

    public async Task<List<Style>> ReadStyleMappingAsync()
    {
        var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "genre-mappings.json");

        var styles = await JsonSerializer.DeserializeAsync<List<Style>>(File.OpenRead(jsonPath), _jsonOptions);

        if (styles is not null) return styles;
        logger.LogWarning("{Message}", $"Style mapping file not found");
        throw new FileNotFoundException("Style mapping file not found");
    }
}