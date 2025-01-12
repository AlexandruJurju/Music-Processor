using Music_Processor.Interfaces;
using Music_Processor.Services;

namespace Music_Processor.Factories;

public class MetadataHandlesFactory
{
    private readonly Dictionary<string, IMetadataHandler> _extractors;

    public MetadataHandlesFactory()
    {
        _extractors = new Dictionary<string, IMetadataHandler>(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp3", new MP3MetadataHandler() },
            { ".flac", new FlacMetadataHandler() }
        };
    }

    public IMetadataHandler GetExtractor(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (_extractors.TryGetValue(extension, out var extractor))
        {
            return extractor;
        }

        throw new NotSupportedException($"Unsupported file type: {extension}");
    }
}