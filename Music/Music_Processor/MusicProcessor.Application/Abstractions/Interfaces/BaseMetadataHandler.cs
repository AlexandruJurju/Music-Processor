using MusicProcessor.Domain.Entities;
using File = TagLib.File;

namespace MusicProcessor.Application.Abstractions.Interfaces;

public abstract class BaseMetadataHandler : IMetadataHandler
{
    protected abstract string FileType { get; }

    public Song ExtractMetadata(string songPath)
    {
        using var file = File.Create(songPath);
        var tag = file.Tag;

        var metadata = new Song(
            title: tag.Title ?? Path.GetFileNameWithoutExtension(songPath),
            album: tag.Album ?? string.Empty,
            year: (int)tag.Year,
            comment: tag.Comment ?? string.Empty,
            trackNumber: (int)tag.Track,
            duration: file.Properties.Duration,
            fileType: FileType
        );

        // Create Artist entities
        foreach (var performerName in tag.Performers)
        {
            metadata.Artists.Add(new Artist { Name = performerName });
        }

        // Create Genre entities
        foreach (var genreName in tag.Genres)
        {
            metadata.Genres.Add(new Genre { Name = genreName });
        }

        // Create Style entities
        foreach (var styleName in ExtractStyles(file))
        {
            metadata.Styles.Add(new Style { Name = styleName });
        }

        return metadata;
    }

    public abstract void WriteMetadata(string songPath, Song song);
    protected abstract List<string> ExtractStyles(File file);
}