using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Interfaces;

namespace MusicProcessor.Application.Services;

public class MetadataService(
    IEnumerable<IMetadataStrategy> strategies,
    ILogger<MetadataService> logger)
{
    public IMetadataStrategy GetStrategy(string filePath)
    {
        var strategy = strategies.FirstOrDefault(s => s.CanHandle(filePath));

        if (strategy == null)
        {
            var extension = Path.GetExtension(filePath);
            logger.LogError("No metadata strategy found for file type: {Extension}", extension);
            throw new NotSupportedException($"Unsupported file type: {extension}");
        }

        return strategy;
    }
}