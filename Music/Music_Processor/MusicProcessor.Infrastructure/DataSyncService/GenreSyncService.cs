using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.GenreCategories;
using MusicProcessor.Domain.Entities.Genres;
using MusicProcessor.Domain.Models.Config;

namespace MusicProcessor.Infrastructure.DataSyncService;

public class GenreSyncService : IGenreSyncService
{
    private readonly IFileService _fileService;
    private readonly IGenreRepository _genreRepository;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly ILogger<GenreSyncService> _logger;

    public GenreSyncService(IFileService fileService,
        IGenreRepository genreRepository,
        ILogger<GenreSyncService> logger)
    {
        _fileService = fileService;
        _genreRepository = genreRepository;
        _logger = logger;
    }

    public async Task WriteStyleMappingAsync()
    {
        var jsonPath = Path.Combine(_fileService.GetPlaylistsPath(), "genre-mappings.json");

        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
            _logger.LogInformation($"Deleted existing mapping file at {jsonPath}");
        }

        var genres = await _genreRepository.GetAllAsync();
        _logger.LogInformation($"Loaded {genres.Count} genres");

        var styleMappings = genres.Select(genre => new GenreMappingDTO(
            genre.Name,
            genre.RemoveFromSongs,
            genre.GenreCategories.Select(g => g.Name).ToList()
        )).ToList();

        var jsonString = JsonSerializer.Serialize(styleMappings, _jsonOptions);
        await File.WriteAllTextAsync(jsonPath, jsonString);
        _logger.LogInformation($"Written the mapping file to {jsonPath}");
    }

    public async Task<IEnumerable<Genre>> ReadStyleMappingAsync()
    {
        try
        {
            var jsonPath = Path.Combine(_fileService.GetPlaylistsPath(), "genre-mappings.json");

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

            _logger.LogWarning("Genre mapping file not found");
            throw new FileNotFoundException("Genre mapping file not found");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading genre mapping file: {ex.Message}");
            throw;
        }
    }
}