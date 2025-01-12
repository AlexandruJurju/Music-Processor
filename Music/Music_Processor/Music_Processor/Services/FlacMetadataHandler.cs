using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib;


namespace Music_Processor.Services;

public class FlacMetadataHandler : IMetadataHandler
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
            var styleFields = xiph.GetField("Styles");
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

    public void WriteMetadata(string filePath, AudioMetadata audioMetadata)
    {
        try
        {
            using var file = TagLib.File.Create(filePath);
            var tag = (file.Tag as TagLib.Ogg.XiphComment) ?? throw new InvalidOperationException("Could not get Xiph Comment tag");

            // Clear existing genre tags
            tag.Genres = [];

            // Set new genres if any
            if (audioMetadata.Genres.Any())
            {
                tag.Genres = audioMetadata.Genres.ToArray();
            }

            // Remove existing STYLE fields
            tag.RemoveField("Styles");

            // Add new styles if any
            if (audioMetadata.Styles.Any())
            {
                foreach (var style in audioMetadata.Styles)
                {
                    tag.SetField("Styles", style);
                }
            }

            file.Save();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}