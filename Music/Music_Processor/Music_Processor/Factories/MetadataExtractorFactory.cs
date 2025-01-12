using Music_Processor.Interfaces;
using Music_Processor.Services;

namespace Music_Processor.Factories;

public class MetadataExtractorFactory
{
    private readonly Dictionary<string, IMetadataExtractor> _extractors;

    public MetadataExtractorFactory()
    {
        _extractors = new Dictionary<string, IMetadataExtractor>(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp3", new MP3MetadataExtractor() },
            { ".flac", new FlacMetadataExtractor() }
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