using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Model;

namespace MusicProcessor.Infrastructure.SerializationStrategies;

public class JsonSerializationStrategy : IMetadataSerializationStrategy
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IFileService _fileService;
    private readonly ILogger<JsonSerializationStrategy> _logger;

    public JsonSerializationStrategy(ILogger<JsonSerializationStrategy> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public async Task SaveMetadataAsync(List<AudioMetadata> metadata, string playlistName)
    {
        try
        {
            var metadataFile = Path.Combine(_fileService.GetPlaylistsPath(), playlistName + ".json");
            var json = JsonSerializer.Serialize(metadata, _jsonOptions);
            await File.WriteAllTextAsync(metadataFile, json);
            _logger.LogInformation("Metadata saved to: {OutputPath}", metadataFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving metadata to JSON");
            throw;
        }
    }

    public async Task<List<AudioMetadata>> LoadMetadataAsync(string playlistName)
    {
        try
        {
            var metadataFile = Path.Combine(_fileService.GetPlaylistsPath(), playlistName + ".json");
            _logger.LogInformation($"Reading metadata from: {metadataFile}");
            var json = await File.ReadAllTextAsync(metadataFile);
            var metadata = JsonSerializer.Deserialize<List<AudioMetadata>>(json, _jsonOptions);
            return metadata ?? new List<AudioMetadata>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading metadata from JSON");
            throw;
        }
    }

    public bool CanHandle(string filePath)
    {
        return Path.GetExtension(filePath).ToLower() == ".json";
    }
}