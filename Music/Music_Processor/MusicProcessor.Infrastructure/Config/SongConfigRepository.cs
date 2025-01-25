using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;

namespace MusicProcessor.Infrastructure.Config;

public class SongConfigRepository(
    IFileService fileService,
    ISongRepository songRepository,
    ILogger<SongConfigRepository> logger) : ISongConfigRepository
{
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task WriteSongsAsync()
    {
        var jsonPath = Path.Combine(fileService.GetPlaylistsPath(), "songs.json");

        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
            logger.LogInformation("{Message}", $"Deleted existing song file at {jsonPath}");
        }
        
        
    }
}