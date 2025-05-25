using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Songs;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace MusicProcessor.Infrastructure.MetadataService;

public class MetadataService : IMetadataService
{
    private const string Styles = "STYLES";
    private const string Artists = "ARTISTS";

    public Song ReadMetadata(string filePath)
    {
        using var file = File.Create(filePath);
        Tag tag = file.Tag;

        var song = new Song
        {
            Title = tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
            Isrc = tag.ISRC,
            Artists = ReadArtists(file),
            Artist = tag.FirstPerformer,
            Styles = ReadStyles(file),
            Genres = ReadGenres(file),
            DiscNumber = (int)tag.Disc,
            DiscCount = (int)tag.DiscCount,
            AlbumName = tag.Album,
            Duration = file.Properties.Duration.Seconds,
            Year = tag.Year,
            TrackNumber = (int)tag.Track,
            TracksCount = (int)tag.TrackCount
        };

        return song;
    }

    private List<string> ReadArtists(File file)
    {
        if (!(file.GetTag(TagTypes.Xiph) is XiphComment xiphTag))
        {
            return new();
        }

        string[]? artists = xiphTag.GetField(Artists);
        if (artists == null || artists.Length == 0)
        {
            return new();
        }

        return artists.ToList();
    }

    private List<string> ReadStyles(File file)
    {
        if (!(file.GetTag(TagTypes.Xiph) is XiphComment xiphTag))
        {
            return new();
        }

        string[]? artists = xiphTag.GetField(Styles);
        if (artists == null || artists.Length == 0)
        {
            return new();
        }

        return artists.ToList();
    }

    private List<string> ReadGenres(File file)
    {
        string[] genres = file.Tag.Genres;
        return genres.ToList();
    }

    public void UpdateSongMetadata(Song song, string songPath)
    {
        using var file = File.Create(songPath);

        file.Tag.Title = song.Title;
        file.Tag.Album = song.AlbumName;
        file.Tag.Year = song.Year;
        file.Tag.AlbumArtists = [song.Artist];

        WriteGenres(song, file);
        WriteStyles(song, file);
        WriteArtists(song, file);

        file.Save();
    }

    private void WriteArtists(Song song, File file)
    {
        List<string> artists = song.Artists;

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Artists, artists.ToArray());
        }
    }

    private void WriteStyles(Song song, File file)
    {
        List<string> styles = song.Styles;

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Styles, styles.ToArray());
        }
    }

    private void WriteGenres(Song song, File file)
    {
        List<string> genres = song.Genres;

        if (genres.Any())
        {
            file.Tag.Genres = genres.ToArray();
        }
    }
}
