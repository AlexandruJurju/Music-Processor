using System.Text.Json;
using Microsoft.Extensions.Logging;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services;

public class JsonMetadataService : IMetadataService
{
    private readonly MetadataHandlesFactory _handlesFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<JsonMetadataService> _logger;

    public JsonMetadataService(ILogger<JsonMetadataService> logger)
    {
        _handlesFactory = new MetadataHandlesFactory();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<AudioMetadata>> ProcessDirectoryAsync(string directoryPath, bool recursive = false)
    {
        var metadata = new List<AudioMetadata>();
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var files = Directory.GetFiles(directoryPath, "*.*", searchOption)
            .Where(f => Constants.Constants.ProcessableAudioFileFormats.Contains(Path.GetExtension(f).ToLower()));

        foreach (var file in files)
        {
            try
            {
                var extractor = _handlesFactory.GetExtractor(file);
                var audioMetadata = extractor.ExtractMetadata(file);
                metadata.Add(audioMetadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file: {FilePath}", file);
            }
        }

        return metadata;
    }

    public async Task SaveMetadataToJsonAsync(List<AudioMetadata> metadata, string outputJsonPath)
    {
        try
        {
            var json = JsonSerializer.Serialize(metadata, _jsonOptions);
            await File.WriteAllTextAsync(outputJsonPath, json);
            _logger.LogInformation("Metadata saved to: {OutputPath}", outputJsonPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving metadata to JSON");
            throw;
        }
    }

    public async Task<List<AudioMetadata>> LoadMetadataFromJsonAsync(string jsonPath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(jsonPath);
            var metadata = JsonSerializer.Deserialize<List<AudioMetadata>>(json, _jsonOptions);
            return metadata ?? new List<AudioMetadata>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading metadata from JSON");
            throw;
        }
    }
}