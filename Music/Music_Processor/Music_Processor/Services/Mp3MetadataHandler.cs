using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using Tag = TagLib.Tag;

namespace Music_Processor.Services;

public class MP3MetadataHandler : IMetadataHandler
{
    public AudioMetadata ExtractMetadata(string filePath)
    {
        using var file = File.Create(filePath);
        var tag = file.GetTag(TagTypes.Id3v2, true) as TagLib.Id3v2.Tag;

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

    public void WriteMetadata(string filePath, AudioMetadata audioMetadata)
    {
        // todo: fix memory problems
        using var file = File.Create(filePath);

        // Get or create ID3v2 tag
        var tag = file.GetTag(TagTypes.Id3v2, true) as TagLib.Id3v2.Tag;
        if (tag == null)
        {
            throw new InvalidOperationException("Failed to create ID3v2 tag");
        }

        // Remove existing genre frames
        tag.RemoveFrames("TCON");

        // Add new genre frame if there are genres
        if (audioMetadata.Genres.Any())
        {
            tag.Genres = audioMetadata.Genres.ToArray();
        }

        // Remove existing style frames first
        var existingStyleFrames = tag.GetFrames("TXXX")
            .OfType<UserTextInformationFrame>()
            .Where(f => f.Description == "Styles")
            .ToList();

        foreach (var frame in existingStyleFrames)
        {
            tag.RemoveFrame(frame);
        }

        // Add new style frame if there are styles
        if (audioMetadata.Styles.Any())
        {
            var styleFrame = new UserTextInformationFrame("TXXX")
            {
                Description = "Styles",
                Text = audioMetadata.Styles.ToArray(),
                TextEncoding = StringType.UTF8
            };
            tag.AddFrame(styleFrame);
        }

        // todo: fix memory problems
        file.Save();
    }

    private List<string> ExtractStyles(Tag tag)
    {
        var styles = new List<string>();

        if (tag is TagLib.Id3v2.Tag id3v2)
        {
            var styleFrame = id3v2.GetFrames()
                .OfType<UserTextInformationFrame>()
                .FirstOrDefault(f => f.Description == "Styles");

            if (styleFrame != null)
            {
                styles.AddRange(styleFrame.Text);
            }
        }

        return styles;
    }
}