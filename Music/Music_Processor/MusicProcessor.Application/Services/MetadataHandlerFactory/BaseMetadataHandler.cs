using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Entities;
using File = TagLib.File;

namespace MusicProcessor.Application.Services.MetadataHandlerFactory;

public abstract class BaseMetadataHandler(ILogger logger) : IMetadataHandler
{
    protected readonly ILogger _logger = logger;
    protected abstract string FileType { get; }

    public Song ReadMetadata(string songPath)
    {
        using var file = File.Create(songPath);
        var tag = file.Tag;

        var metadata = new Song(
            songPath,
            tag.Title ?? Path.GetFileNameWithoutExtension(songPath),
            tag.Album != null ? new Album(tag.Album) : null,
            (int)tag.Year,
            tag.Comment ?? string.Empty,
            (int)tag.Track,
            file.Properties.Duration,
            FileType
        );

        // Create Artist entities
        foreach (var performer in tag.Performers)
        {
            // because of spotdl data -> reads performers with / between their names
            var splitPerformers = performer.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var performerName in splitPerformers) metadata.Artists.Add(new Artist { Name = performerName });
        }

        // cant read genres like this, they have to be tied to a style
        // Create Genre entities
        // foreach (var genreName in tag.Genres) metadata.Genres.Add(new Genre { Name = genreName });

        // Create Style entities
        foreach (var styleName in ExtractStyles(file)) metadata.Styles.Add(new Style { Name = styleName });

        return metadata;
    }

    public abstract void WriteMetadata(Song song);

    public bool CanHandle(string filePath)
    {
        return Path.GetExtension(filePath).Equals(FileType, StringComparison.OrdinalIgnoreCase);
    }

    protected abstract List<string> ExtractStyles(File file);
}