using Music_Processor.Interfaces;
using Music_Processor.Model;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace Music_Processor.Services;

public class FlacMetadataHandler : IMetadataHandler
{
    public AudioMetadata ExtractMetadata(string filePath)
    {
        using var file = File.Create(filePath);
        var tag = file.Tag;

        var styles = ExtractStyles(file);

        return new AudioMetadata
        {
            FilePath = filePath,
            FileType = "FLAC",
            Title = tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
            Artists = tag.Performers.ToList(),
            Album = tag.Album ?? string.Empty,
            Genres = tag.Genres.ToList(),
            Styles = styles,
            Year = (int)tag.Year,
            Comment = tag.Comment ?? string.Empty,
            TrackNumber = (int)tag.Track,
            Duration = file.Properties.Duration
        };
    }

    public void WriteMetadata(string filePath, AudioMetadata audioMetadata)
    {
        try
        {
            using var file = File.Create(filePath);
            var tag = file.Tag;

            // Handle the combined tag first for genres
            tag.Genres = audioMetadata.Genres.ToArray();

            // Then specifically handle the Xiph tag for styles
            if (file.TagTypes.HasFlag(TagTypes.Xiph))
            {
                var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
                if (xiphTag != null)
                {
                    // Clear existing style fields
                    xiphTag.RemoveField("STYLE");
                    xiphTag.RemoveField("Style");
                    xiphTag.RemoveField("Styles");
                    xiphTag.RemoveField("styles");

                    // Add new styles
                    if (audioMetadata.Styles.Any())
                    {
                        xiphTag.SetField("STYLE", audioMetadata.Styles.ToArray());
                    }
                }
            }

            file.Save();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error writing metadata to FLAC file {filePath}: {ex.Message}", ex);
        }
    }

    private List<string> ExtractStyles(File file)
    {
        var styles = new List<string>();

        // Try to get the Xiph comment tag from the combined tags
        if (file.TagTypes.HasFlag(TagTypes.Xiph))
        {
            var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
            if (xiphTag != null)
            {
                var styleFields = xiphTag.GetField("STYLE");
                styles.AddRange(styleFields);
            }
        }

        return styles.Distinct().ToList();
    }
}