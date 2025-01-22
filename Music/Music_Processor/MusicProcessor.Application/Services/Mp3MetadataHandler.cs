using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using FileTypes = MusicProcessor.Domain.Constants.FileTypes;
using Tag = TagLib.Id3v2.Tag;

namespace MusicProcessor.Application.Services;

public class MP3MetadataHandler(ILogger<MP3MetadataHandler> logger) : BaseMetadataStrategy(logger)
{
    protected override string FileType => FileTypes.MP3;

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
                logger.LogDebug("Extracted {Count} styles from MP3 file", styleFrame.Text.Length);
            }
            else
            {
                logger.LogDebug("No styles frame found in MP3 file");
            }
        }
        else
        {
            logger.LogDebug("No ID3v2 tag found in MP3 file");
        }

        return styles.Distinct().ToList();
    }

    public override void UpdateMetadata(Song song)
    {
        logger.LogInformation("Updating metadata for MP3 file: {FilePath}", song.FilePath);
        try
        {
            using var file = File.Create(song.FilePath);
            var tag = file.GetTag(TagTypes.Id3v2, true) as Tag;
            if (tag == null)
            {
                logger.LogError("Failed to create ID3v2 tag for file: {FilePath}", song.FilePath);
                throw new InvalidOperationException("Failed to create ID3v2 tag");
            }

            logger.LogDebug("Removing existing genre frames");
            tag.RemoveFrames("TCON");

            if (song.Genres.Any())
            {
                var genres = song.Genres.Select(g => g.Name).ToArray();
                logger.LogDebug("Setting {Count} genres: {Genres}", genres.Length, string.Join(", ", genres));
                tag.Genres = genres;
            }
            else
            {
                logger.LogDebug("No genres to set");
            }

            var existingStyleFrames = tag.GetFrames("TXXX")
                .OfType<UserTextInformationFrame>()
                .Where(f => f.Description == "Styles")
                .ToList();

            logger.LogDebug("Removing {Count} existing style frames", existingStyleFrames.Count);
            foreach (var frame in existingStyleFrames)
            {
                tag.RemoveFrame(frame);
            }

            if (song.Styles.Any())
            {
                var styles = song.Styles.Where(s => !s.RemoveFromSongs).Select(s => s.Name).ToArray();
                logger.LogDebug("Setting {Count} styles: {Styles}", styles.Length, string.Join(", ", styles));
                var styleFrame = new UserTextInformationFrame("TXXX")
                {
                    Description = "Styles",
                    Text = styles,
                    TextEncoding = StringType.UTF8
                };
                tag.AddFrame(styleFrame);
            }
            else
            {
                logger.LogDebug("No styles to set");
            }

            file.Save();
            logger.LogInformation("Successfully updated metadata for MP3 file: {FilePath}", song.FilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating metadata for MP3 file {FilePath}", song.FilePath);
            throw;
        }
    }
}