using System.Text.Json;
using Microsoft.Extensions.Logging;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services;

public class MetadataService : IMetadataService
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<MetadataService> _logger;
    private readonly MetadataHandlesFactory _metadataHandlesFactory;

    public MetadataService(ILogger<MetadataService> logger)
    {
        _metadataHandlesFactory = new MetadataHandlesFactory();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public List<AudioMetadata> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false)
    {
        var metadata = new List<AudioMetadata>();
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var files = Directory.GetFiles(directoryPath, "*.*", searchOption)
            .Where(f => Constants.Constants.ProcessableAudioFileFormats.Contains(Path.GetExtension(f).ToLower()));

        foreach (var file in files)
        {
            try
            {
                var extractor = _metadataHandlesFactory.GetHandler(file);
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

    public void WriteSongMetadata(string songPath, AudioMetadata audioMetadata)
    {
        var handle = _metadataHandlesFactory.GetHandler(songPath);
        handle.WriteMetadata(songPath, audioMetadata);
    }

    public AudioMetadata ExtractMetadataFromSong(string songPath)
    {
        var handle = _metadataHandlesFactory.GetHandler(songPath);
        return handle.ExtractMetadata(songPath);
    }
}