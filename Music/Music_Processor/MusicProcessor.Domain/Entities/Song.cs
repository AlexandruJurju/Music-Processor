using System.Security.Cryptography;
using System.Text;
using MusicProcessor.Domain.Common;

namespace MusicProcessor.Domain.Entities;

public sealed class Song : BaseEntity
{
    public Song()
    {
    }

    public Song(
        string title,
        string album,
        int? year,
        string comment,
        int trackNumber,
        TimeSpan duration,
        string fileType)
    {
        Title = title;
        Album = album;
        Year = year;
        Comment = comment;
        TrackNumber = trackNumber;
        Duration = duration;
        FileType = fileType;
        MetadataHash = ComputeHash();
    }

    public string Title { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public int? Year { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int TrackNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string MetadataHash { get; set; } = string.Empty;

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Style> Styles { get; set; } = new List<Style>();

    public string ComputeHash()
    {
        var hashString = string.Join("|",
            Title,
            string.Join(",", Artists.Select(a => a.Name).OrderBy(a => a)),
            Album,
            string.Join(",", Genres.Select(g => g.Name).OrderBy(g => g)),
            string.Join(",", Styles.Select(s => s.Name).OrderBy(s => s)),
            Year?.ToString() ?? "",
            Comment,
            TrackNumber,
            Duration.ToString(),
            FileType
        );

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
        return Convert.ToBase64String(hashBytes);
    }
}