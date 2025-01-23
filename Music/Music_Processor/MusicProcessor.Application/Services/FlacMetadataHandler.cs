using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;
using FileTypes = MusicProcessor.Domain.Constants.FileTypes;

namespace MusicProcessor.Application.Services;

public class FlacMetadataHandler(ILogger<FlacMetadataHandler> logger) : BaseMetadataStrategy(logger)
{
    protected override string FileType => FileTypes.FLAC;

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
                logger.LogDebug("Extracted {Count} styles from FLAC file", styleFields.Length);
            }
            else
            {
                logger.LogDebug("No Xiph comment tag found in FLAC file");
            }
        }
        else
        {
            logger.LogDebug("No Xiph tags found in FLAC file");
        }

        return styles.Distinct().ToList();
    }

    public override void UpdateMetadata(Song song)
    {
        logger.LogInformation("Updating metadata for FLAC file: {FilePath}", song.FilePath);
        try
        {
            using var file = File.Create(song.FilePath);
            var tag = file.Tag;

            var genres = song.Styles.SelectMany(s => s.Genres.Select(g => g.Name)).ToArray();
            logger.LogDebug("Setting {Count} genres: {Genres}", genres.Length, string.Join(", ", genres));
            tag.Genres = genres;

            if (file.TagTypes.HasFlag(TagTypes.Xiph))
            {
                var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
                if (xiphTag != null)
                {
                    logger.LogDebug("Clearing existing style fields");
                    xiphTag.RemoveField("STYLE");
                    xiphTag.RemoveField("Style");
                    xiphTag.RemoveField("Styles");
                    xiphTag.RemoveField("styles");

                    if (song.Styles.Any())
                    {
                        var styles = song.Styles.Where(s => !s.RemoveFromSongs).Select(s => s.Name).ToArray();
                        logger.LogDebug("Setting {Count} styles: {Styles}", styles.Length, string.Join(", ", styles));
                        xiphTag.SetField("STYLE", styles);
                    }
                    else
                    {
                        logger.LogDebug("No styles to set");
                    }
                }
                else
                {
                    logger.LogWarning("Failed to get Xiph comment tag for FLAC file");
                }
            }
            else
            {
                logger.LogWarning("No Xiph tags found in FLAC file for writing styles");
            }

            file.Save();
            logger.LogInformation("Successfully updated metadata for FLAC file: {FilePath}", song.FilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing metadata to FLAC file {FilePath}", song.FilePath);
            throw new Exception($"Error writing metadata to FLAC file {song.FilePath}: {ex.Message}", ex);
        }
    }
}