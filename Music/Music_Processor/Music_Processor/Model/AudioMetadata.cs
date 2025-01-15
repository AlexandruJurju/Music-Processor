using System.Security.Cryptography;
using System.Text;

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
    public string MetadataHash { get; set; } = string.Empty;


    public AudioMetadata()
    {
    }

    public AudioMetadata(
        string filePath,
        string title,
        List<string> artists,
        string album,
        List<string> genres,
        List<string> styles,
        int? year,
        string comment,
        int trackNumber,
        TimeSpan duration,
        string fileType)
    {
        FilePath = filePath;
        Title = title;
        Artists = artists;
        Album = album;
        Genres = genres;
        Styles = styles;
        Year = year;
        Comment = comment;
        TrackNumber = trackNumber;
        Duration = duration;
        FileType = fileType;
        MetadataHash = ComputeHash();
    }

    public string ComputeHash()
    {
        var hashString = string.Join("|",
            Title,
            string.Join(",", Artists.OrderBy(a => a)),
            Album,
            string.Join(",", Genres.OrderBy(g => g)),
            string.Join(",", Styles.OrderBy(s => s)),
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