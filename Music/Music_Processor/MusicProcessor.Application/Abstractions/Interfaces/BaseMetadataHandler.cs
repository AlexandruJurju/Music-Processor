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
            songPath,
            tag.Title ?? Path.GetFileNameWithoutExtension(songPath),
            tag.Album ?? string.Empty,
            (int)tag.Year,
            tag.Comment ?? string.Empty,
            (int)tag.Track,
            file.Properties.Duration,
            FileType
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
