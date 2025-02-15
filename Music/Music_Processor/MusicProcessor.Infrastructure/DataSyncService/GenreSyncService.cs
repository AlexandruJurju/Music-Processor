using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities;
using MusicProcessor.Domain.Models.Config;

namespace MusicProcessor.Infrastructure.DataSyncService;

public class GenreSyncService(
    IFileService fileService,
    IGenreRepository genreRepository,
    ILogger<GenreSyncService> logger
) : IGenreSyncService
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

        var genres = await genreRepository.GetAllAsync();
        logger.LogInformation("{Message}", $"Loaded {genres.Count} genres");

        var styleMappings = genres.Select(genre => new GenreMappingDTO(
            genre.Name,
            genre.RemoveFromSongs,
            genre.GenreCategories.Select(g => g.Name).ToList()
        )).ToList();

        var jsonString = JsonSerializer.Serialize(styleMappings, _jsonOptions);
        await File.WriteAllTextAsync(jsonPath, jsonString);
        logger.LogInformation("{Message}", $"Written the mapping file to {jsonPath}");
    }

    public async Task<IEnumerable<Genre>> ReadStyleMappingAsync()
    {
        try
        {
            var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "genre-mappings.json");

            var genreDTOs = await JsonSerializer.DeserializeAsync<List<GenreMappingDTO>>(File.OpenRead(jsonPath), _jsonOptions);

            if (genreDTOs is not null)
            {
                return genreDTOs.Select(dto => new Genre
                {
                    Name = dto.genreName,
                    RemoveFromSongs = dto.RemoveFromSongs,
                    GenreCategories = dto.genreCategoryNames.Select(name => new GenreCategory(name)).ToList()
                });
            }

            logger.LogWarning("{Message}", "Genre mapping file not found");
            throw new FileNotFoundException("Genre mapping file not found");
        }
        catch (Exception ex)
        {
            logger.LogInformation("{Message}", "Error reading genre mapping file");
            throw;
        }
    }
}