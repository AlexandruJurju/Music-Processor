namespace MusicProcessor.Domain.Models.SpotDL.Download;

public record SyncSummary
{
    public List<string> LookupErrors { get; } = new();
    public List<string> DownloadErrors { get; } = new();
    public int TotalMissingSongs => LookupErrors.Count + DownloadErrors.Count;
}