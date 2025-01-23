using System.Security.Cryptography;
using System.Text;
using MusicProcessor.Domain.Common;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.Domain.Entities;

public sealed class Song : BaseEntity
{
    public Song()
    {
    }

    public Song(
        string filePath,
        string title,
        string album,
        int? year,
        string comment,
        int trackNumber,
        TimeSpan duration,
        string fileType)
    {
        FilePath = filePath;
        Title = title;
        Album = album;
        Year = year;
        Comment = comment;
        TrackNumber = trackNumber;
        Duration = duration;
        FileType = ValidateFileType(fileType);
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
    public string FilePath { get; set; } = string.Empty;

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public ICollection<Style> Styles { get; set; } = new List<Style>();

    private string ComputeHash()
    {
        var hashString = string.Join("|",
            FilePath,
            Title,
            string.Join(",", Artists.Select(a => a.Name).OrderBy(a => a)),
            Album,
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

    private string ValidateFileType(string fileType)
    {
        var allowedFileTypes = new[] { FileTypes.MP3, FileTypes.FLAC };
        if (!allowedFileTypes.Contains(fileType))
        {
            throw new ArgumentException($"Invalid file type. Allowed values are: {string.Join(", ", allowedFileTypes)}");
        }

        return fileType;
    }
}