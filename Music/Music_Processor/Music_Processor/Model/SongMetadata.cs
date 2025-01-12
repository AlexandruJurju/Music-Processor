namespace Music_Processor.Model;

public record SongMetadata(
    IEnumerable<string> Artists,
    string Name,
    IEnumerable<string> Genres,
    IEnumerable<string> Styles
);