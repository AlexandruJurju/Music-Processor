using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace MusicProcessor.Application.Services;

public class FlacMetadataHandler : BaseMetadataHandler
{
    protected override string FileType => "FLAC";

    protected override List<string> ExtractStyles(File file)
    {
        var styles = new List<string>();

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

    public override void UpdateMetadata(Song song)
    {
        try
        {
            using var file = File.Create(song.FilePath);
            var tag = file.Tag;

            // Handle the combined tag first for genres
            tag.Genres = song.Genres.Select(g => g.Name).ToArray();

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
                    if (song.Styles.Any())
                    {
                        xiphTag.SetField("STYLE", song.Styles.Select(s => s.Name).ToArray());
                    }
                }
            }

            file.Save();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error writing metadata to FLAC file {song.FilePath}: {ex.Message}", ex);
        }
    }
}