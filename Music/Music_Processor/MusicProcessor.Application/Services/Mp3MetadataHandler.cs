using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using Tag = TagLib.Id3v2.Tag;

namespace MusicProcessor.Application.Services;

public class MP3MetadataHandler : BaseMetadataHandler
{
    protected override string FileType => "MP3";

    protected override List<string> ExtractStyles(File file)
    {
        var styles = new List<string>();
        var tag = file.GetTag(TagTypes.Id3v2, true) as Tag;

        if (tag is Tag id3v2)
        {
            var styleFrame = id3v2.GetFrames()
                .OfType<UserTextInformationFrame>()
                .FirstOrDefault(f => f.Description == "Styles");

            if (styleFrame != null)
            {
                styles.AddRange(styleFrame.Text);
            }
        }

        return styles.Distinct().ToList();
    }

    public override void WriteMetadata(string songPath, Song song)
    {
        using var file = File.Create(songPath);
        var tag = file.GetTag(TagTypes.Id3v2, true) as Tag;
        if (tag == null)
        {
            throw new InvalidOperationException("Failed to create ID3v2 tag");
        }

        // Remove existing genre frames
        tag.RemoveFrames("TCON");

        // Add new genre frame if there are genres
        if (song.Genres.Any())
        {
            tag.Genres = song.Genres.Select(g => g.Name).ToArray();
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
        if (song.Styles.Any())
        {
            var styleFrame = new UserTextInformationFrame("TXXX")
            {
                Description = "Styles",
                Text = song.Styles.Select(s => s.Name).ToArray(),
                TextEncoding = StringType.UTF8
            };
            tag.AddFrame(styleFrame);
        }

        file.Save();
    }
}