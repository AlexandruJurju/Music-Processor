using Microsoft.Extensions.Logging;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;
using FileTypes = MusicProcessor.Domain.Constants.FileTypes;

namespace MusicProcessor.Application.Services.MetadataHandlerFactory;

// todo: update for rename
public class FlacMetadataHandler(ILogger<FlacMetadataHandler> logger) : BaseMetadataHandler(logger)
{
    protected override string FileType => FileTypes.FLAC;

    protected override List<string> ExtractGenres(File file)
    {
        var genres = new List<string>();

        if (file.TagTypes.HasFlag(TagTypes.Xiph))
        {
            var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
            if (xiphTag != null)
            {
                var genreFields = xiphTag.GetField("GENRE");
                genres.AddRange(genreFields);
                logger.LogDebug("Extracted {Count} genres from FLAC file", genreFields.Length);
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

        return genres.Distinct().ToList();
    }

    // todo; this is wrong
    public override void WriteMetadata(Song song)
    {
        logger.LogInformation("Updating metadata for FLAC file: {FilePath}", song.FilePath);
        try
        {
            using var file = File.Create(song.FilePath);
            var tag = file.Tag;

            var genres = song.Genres.SelectMany(s => s.GenreCategories.Select(g => g.Name)).ToArray();
            logger.LogDebug("Setting {Count} genre categories: {GenreCategories}", genres.Length, string.Join(", ", genres));
            tag.Genres = genres;

            if (file.TagTypes.HasFlag(TagTypes.Xiph))
            {
                var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
                if (xiphTag != null)
                {
                    logger.LogDebug("Clearing existing style fields");
                    xiphTag.RemoveField("Genre");
                    xiphTag.RemoveField("GenreCategories");

                    if (song.Genres.Any())
                    {
                        var styles = song.Genres.Where(s => !s.RemoveFromSongs).Select(s => s.Name).ToArray();
                        logger.LogDebug("Setting {Count} styles: {GenreCategories}", styles.Length, string.Join(", ", styles));
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