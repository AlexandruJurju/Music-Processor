using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Music_Processor.Constants;
using Music_Processor.Interfaces;
using Music_Processor.Model;

namespace Music_Processor.Services;

public class XmlSerializationStrategy : IMetadataSerializationStrategy
{
    private readonly ILogger<XmlSerializationStrategy> _logger;
    private readonly XmlSerializer _serializer;

    public XmlSerializationStrategy(XmlSerializer serializer, ILogger<XmlSerializationStrategy> logger)
    {
        _serializer = serializer;
        _logger = logger;
    }

    public bool CanHandle(string filePath)
    {
        return Path.GetExtension(filePath).ToLower() == ".xml";
    }

    public async Task SaveMetadataAsync(List<AudioMetadata> metadata, string playlistName)
    {
        // try
        // {
        //     var metadataFile = Path.Combine(AppPaths.ExecutableDirectory, playlistName + ".xml");
        //     await using var stream = File.Create(metadataFile);
        //     _serializer.Serialize(stream, metadata);
        //     _logger.LogInformation("Metadata saved to: {OutputPath}", metadataFile);
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Error saving metadata to XML");
        //     throw;
        // }
        throw new NotImplementedException();
    }

    public async Task<List<AudioMetadata>> LoadMetadataAsync(string playlistName)
    {
        // try
        // {
        //     var metadataFile = Path.Combine(AppPaths.ExecutableDirectory, playlistName + ".xml");
        //     await using var stream = File.OpenRead(metadataFile);
        //     var metadata = (List<AudioMetadata>?)_serializer.Deserialize(stream);
        //     return metadata ?? new List<AudioMetadata>();
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Error loading metadata from XML");
        //     throw;
        // }

        throw new NotImplementedException();
    }
}