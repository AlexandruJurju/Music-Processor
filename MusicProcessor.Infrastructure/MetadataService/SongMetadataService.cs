using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Domain.Styles;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace MusicProcessor.Infrastructure.MetadataService;

public class SongMetadataService : ISongMetadataService
{
    private const string Styles = "STYLES";
    private const string Artists = "ARTISTS";

    public Song ReadMetadata(string filePath)
    {
        using var file = File.Create(filePath);

        Tag tag = file.Tag;

        var mainArtist = Artist.Create(tag.FirstAlbumArtist ?? tag.FirstPerformer ?? tag.AlbumArtists[0]);
        var artists = ReadArtists(file).Select(Artist.Create).ToList();
        var styles = ReadStyles(file).Select(style => Style.Create(style, false)).ToList();
        var album = Album.Create(tag.Album, mainArtist);

        return Song.Create(
            title: tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
            mainArtist: mainArtist,
            artists: artists,
            styles: styles,
            album: album,
            discNumber: (int)tag.Disc,
            discCount: (int)tag.DiscCount,
            duration: (int)file.Properties.Duration.TotalSeconds,
            year: tag.Year,
            trackNumber: (int)tag.Track,
            tracksCount: (int)tag.TrackCount,
            isrc: tag.ISRC
        );
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

    // private List<string> ReadGenres(File file)
    // {
    //     string[] genres = file.Tag.Genres;
    //     return genres.ToList();
    // }

    public void UpdateSongMetadata(Song song, string songPath)
    {
        using var file = File.Create(songPath);

        file.Tag.Title = song.Title;
        file.Tag.Album = song.Album.Name;
        file.Tag.Year = song.Year;
        file.Tag.AlbumArtists = [song.MainArtist.Name];

        WriteGenres(song, file);
        WriteStyles(song, file);
        WriteArtists(song, file);

        file.Save();
    }

    private void WriteArtists(Song song, File file)
    {
        var artists = song.Artists.Select(artist => artist.Name).ToList();

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Artists, artists.ToArray());
        }
    }

    private void WriteStyles(Song song, File file)
    {
        var styles = song.Styles.Select(style => style.Name).ToList();

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Styles, styles.ToArray());
        }
    }

    private void WriteGenres(Song song, File file)
    {
        var genres = song.Styles.SelectMany(style => style.Genres.Select(genre => genre.Name)).ToList();

        if (genres.Any())
        {
            file.Tag.Genres = genres.ToArray();
        }
    }
}
