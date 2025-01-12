using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib.Id3v2;
using Tag = TagLib.Tag;

namespace Music_Processor.Services;

public class MP3MetadataExtractor : IMetadataExtractor
{
    public AudioMetadata ExtractMetadata(string filePath)
    {
        using var file = TagLib.File.Create(filePath);
        var tag = file.Tag;

        return new AudioMetadata
        {
            FilePath = filePath,
            FileType = "MP3",
            Title = tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
            Artists = tag.Performers.ToList(),
            Album = tag.Album ?? string.Empty,
            Genres = tag.Genres.ToList(),
            Styles = ExtractStyles(tag),
            Year = (int)tag.Year,
            Comment = tag.Comment ?? string.Empty,
            TrackNumber = (int)tag.Track,
            Duration = file.Properties.Duration
        };
    }

    private List<string> ExtractStyles(Tag tag)
    {
        // First try to get styles from custom TXXX frame
        var styles = new List<string>();

        // Check if we can access ID3v2 tag
        if (tag is TagLib.Id3v2.Tag id3v2)
        {
            var styleFrames = id3v2.GetFrames()
                .OfType<UserTextInformationFrame>()
                .Where(f => f.Description.Equals("Styles", StringComparison.OrdinalIgnoreCase));

            foreach (var frame in styleFrames)
            {
                styles.AddRange(frame.Text);
            }
        }

        // If no styles found in TXXX, try to parse them from comment
        if (!styles.Any() && !string.IsNullOrEmpty(tag.Comment))
        {
            styles.AddRange(tag.Comment.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()));
        }

        return styles;
    }
}