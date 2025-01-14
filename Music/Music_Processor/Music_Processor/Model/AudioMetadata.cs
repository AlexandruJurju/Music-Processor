namespace Music_Processor.Model;

public class AudioMetadata
{
    public string FilePath { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Artists { get; set; } = new();
    public string Album { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<string> Styles { get; set; } = new();
    public int? Year { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public string FileType { get; set; } = string.Empty;
}