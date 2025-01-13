using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib;
using TagLib.Ogg;

namespace Music_Processor.Services;

// todo: fix the tag writing and reading
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

        if (tag is XiphComment xiph)
        {
            // Try to get styles from STYLE field
            var styleFields = xiph.GetField("STYLE");  // FLAC usually uses uppercase field names
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

        return styles.Distinct().ToList();  // Remove any duplicates
    }

    public void WriteMetadata(string filePath, AudioMetadata audioMetadata)
    {
        try
        {
            using var file = TagLib.File.Create(filePath);
            
            // Get or create XiphComment tag
            var tag = file.GetTag(TagTypes.Xiph, true) as XiphComment;
            if (tag == null)
            {
                throw new InvalidOperationException("Failed to create Xiph Comment tag");
            }

            // Clear existing genre tags
            tag.Genres = Array.Empty<string>();

            // Set new genres if any
            if (audioMetadata.Genres.Any())
            {
                tag.Genres = audioMetadata.Genres.ToArray();
            }

            // Remove all variations of style fields (case-sensitive in FLAC)
            tag.RemoveField("STYLE");
            tag.RemoveField("Style");
            tag.RemoveField("Styles");
            tag.RemoveField("styles");

            // Add new styles if any
            if (audioMetadata.Styles.Any())
            {
                // In FLAC/Vorbis comments, we typically use uppercase field names
                // and store each style as a separate field value
                tag.SetField("STYLE", audioMetadata.Styles.ToArray());
            }

            file.Save();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error writing metadata to FLAC file {filePath}: {ex.Message}", ex);
        }
    }
}