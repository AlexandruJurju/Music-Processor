using Microsoft.Extensions.Logging;
using MusicProcessor.Domain.Entities;
using TagLib;
using TagLib.Id3v2;
using File = TagLib.File;
using FileTypes = MusicProcessor.Domain.Constants.FileTypes;
using Tag = TagLib.Id3v2.Tag;

namespace MusicProcessor.Application.Services.MetadataHandlerFactory;

public class MP3MetadataHandler(ILogger<MP3MetadataHandler> logger) : BaseMetadataHandler(logger)
{
    private const string GenreCategoryName = "Category";

    protected override string FileType => FileTypes.MP3;

    protected override List<string> ExtractGenres(File file)
    {
        var genres = new List<string>();
        var tag = file.GetTag(TagTypes.Id3v2, true) as Tag;

        if (tag is { } id3v2)
        {
            genres.AddRange(ExtractGenresFromFrame(id3v2, "TCON"));
            genres.AddRange(ExtractGenresFromFrame(id3v2, "TGEN"));
        }
        else
        {
            logger.LogDebug("No ID3v2 tag found in MP3 file");
        }

        return genres.Distinct().ToList();
    }
    
    private List<string> ExtractGenresFromFrame(Tag id3v2, string frameId)
    {
        var genres = new List<string>();
        var genreFrame = id3v2.GetFrames()
            .OfType<TextInformationFrame>()
            .FirstOrDefault(f => f.FrameId == frameId);

        if (genreFrame != null)
        {
            var genreText = genreFrame.Text.FirstOrDefault() ?? string.Empty;
            var separatedGenres = genreText
                .Split([';', '\0'], StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim())
                .Where(g => !string.IsNullOrWhiteSpace(g));

            genres.AddRange(separatedGenres);
            logger.LogDebug("Extracted {Count} genres from {FrameId} frame: {Genres}", genres.Count, frameId, string.Join(", ", genres));
        }
        else
        {
            logger.LogDebug("No {FrameId} frame found in MP3 file", frameId);
        }

        return genres;
    }

    public override void WriteMetadata(Song song)
    {
        logger.LogInformation("Updating metadata for MP3 file: {FilePath}", song.FilePath);
        try
        {
            using var file = File.Create(song.FilePath);
            var tag = file.GetTag(TagTypes.Id3v2, true) as Tag;

            if (tag == null)
            {
                logger.LogError("Failed to create ID3v2 tag for file: {FilePath}", file.Name);
                throw new InvalidOperationException("Failed to create ID3v2 tag");
            }

            WriteGenres(tag, song.Genres.ToList());
            WriteGenreCategories(tag, song.Genres.ToList());

            file.Save();
            logger.LogInformation("Successfully updated metadata for MP3 file: {FilePath}", song.FilePath);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "File not found: {FilePath}", song.FilePath);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Access denied to file: {FilePath}", song.FilePath);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating metadata for MP3 file {FilePath}", song.FilePath);
            throw;
        }
    }

    private void WriteGenres(Tag tag, List<Genre> genres)
    {
        tag.RemoveFrames("TCON");

        if (genres.Any())
        {
            var genreNames = genres
                .Where(s => !s.RemoveFromSongs)
                .Select(s => s.Name)
                .ToArray();

            logger.LogDebug("Setting {Count} genres: {Genres}", genreNames.Length, string.Join(", ", genreNames));
            var genreFrame = new TextInformationFrame("TCON", StringType.UTF8)
            {
                Text = genreNames
            };

            tag.AddFrame(genreFrame);
        }
        else
        {
            logger.LogDebug("No genres to set");
        }
    }

    private void WriteGenreCategories(Tag tag, List<Genre> genres)
    {
        var existingFrames = tag.GetFrames("TXXX")
            .OfType<UserTextInformationFrame>()
            .Where(f => f.Description == GenreCategoryName)
            .ToList();

        logger.LogDebug("Removing {Count} existing genre category frames", existingFrames.Count);
        foreach (var frame in existingFrames)
        {
            tag.RemoveFrame(frame);
        }

        var genreCategories = genres
            .SelectMany(s => s.GenreCategories.Select(g => g.Name))
            .Distinct()
            .ToArray();

        if (genreCategories.Any())
        {
            logger.LogDebug("Setting {Count} genre categories: {Categories}", genreCategories.Length, string.Join(", ", genreCategories));
            var frame = new UserTextInformationFrame("TXXX")
            {
                Description = GenreCategoryName,
                Text = genreCategories,
                TextEncoding = StringType.UTF8
            };
            tag.AddFrame(frame);
        }
        else
        {
            logger.LogDebug("No genre categories to set");
        }
    }
}