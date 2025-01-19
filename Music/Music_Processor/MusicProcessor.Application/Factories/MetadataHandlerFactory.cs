using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Services;

namespace MusicProcessor.Application.Factories;

public class MetadataHandlerFactory
{
    private readonly Dictionary<string, IMetadataHandler> _handlers;

    public MetadataHandlerFactory()
    {
        _handlers = new Dictionary<string, IMetadataHandler>(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp3", new MP3MetadataHandler() },
            { ".flac", new FlacMetadataHandler() }
        };
    }

    public IMetadataHandler GetHandler(string songFilePath)
    {
        var extension = Path.GetExtension(songFilePath);
        if (_handlers.TryGetValue(extension, out var extractor))
        {
            return extractor;
        }

        throw new NotSupportedException($"Unsupported file type: {extension}");
    }
}