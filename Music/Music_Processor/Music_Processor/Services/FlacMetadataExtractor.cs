using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib;


namespace Music_Processor.Services;

public class FlacMetadataExtractor : IMetadataExtractor
{
    public AudioMetadata ExtractMetadata(string filePath)
    {
        using var file = TagLib.File.Create(filePath);
        var tag = file.Tag;

        return new AudioMetadata
        {
            FilePath = filePath,
            FileType = "FLAC",
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
        var styles = new List<string>();

        if (tag is TagLib.Ogg.XiphComment xiph)
        {
            // Try to get styles from STYLE field
            var styleFields = xiph.GetField("STYLE");
            if (styleFields.Any())
            {
                styles.AddRange(styleFields);
            }
        }

        // If no styles found in STYLE field, try to parse them from comment
        if (!styles.Any() && !string.IsNullOrEmpty(tag.Comment))
        {
            styles.AddRange(tag.Comment.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()));
        }

        return styles;
    }
}