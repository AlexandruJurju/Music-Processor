using Music_Processor.Interfaces;
using Music_Processor.Services;

namespace Music_Processor.Factories;

public class MetadataHandlesFactory
{
    private readonly Dictionary<string, IMetadataExtractor> _extractors;

    public MetadataHandlesFactory()
    {
        _extractors = new Dictionary<string, IMetadataExtractor>(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp3", new MP3MetadataHandler() },
            { ".flac", new FlacMetadataHandler() }
        };
    }

    public IMetadataExtractor GetExtractor(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (_extractors.TryGetValue(extension, out var extractor))
        {
            return extractor;
        }

        throw new NotSupportedException($"Unsupported file type: {extension}");
    }
}