namespace MusicProcessor.Domain.Models.SpotDL.Parse;

public sealed class SpotDLPlaylist
{
    public string Type { get; set; } = string.Empty;
    public List<string> Query { get; set; } = new();
    public List<SpotDLSongMetadata> Songs { get; set; } = new();
}