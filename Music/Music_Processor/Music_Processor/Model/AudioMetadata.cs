namespace Music_Processor.Model;

public record AudioMetadata
{
    public string FilePath { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public List<string> Artists { get; init; } = new();
    public string Album { get; init; } = string.Empty;
    public List<string> Genres { get; init; } = new();
    public List<string> Styles { get; init; } = new();
    public int? Year { get; init; }
    public string Comment { get; init; } = string.Empty;
    public int TrackNumber { get; init; }
    public TimeSpan Duration { get; init; }
    public string FileType { get; init; } = string.Empty;
}