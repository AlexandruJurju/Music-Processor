using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace MusicProcessor.Infrastructure.AudioService;

public class AudioService : IAudioService
{
    private const string Styles = "STYLES";
    private const string Artists = "ARTISTS";

    public Song ReadMetadata(string filePath)
    {
        using var file = File.Create(filePath);

        Tag tag = file.Tag;

        string? mainArtist = tag.FirstAlbumArtist ?? tag.FirstPerformer ?? tag.AlbumArtists[0];
        string[] artists = ReadArtists(file);
        string[] styles = ReadStyles(file);
        string? album = tag.Album;

        return Song.Create(
            title: tag.Title ?? Path.GetFileNameWithoutExtension(filePath),
            mainArtistName: mainArtist,
            artists: artists.ToList(),
            styles: styles.ToList(),
            albumName: album,
            discNumber: (int)tag.Disc,
            discCount: (int)tag.DiscCount,
            duration: (int)file.Properties.Duration.TotalSeconds,
            year: tag.Year,
            trackNumber: (int)tag.Track,
            tracksCount: (int)tag.TrackCount,
            isrc: tag.ISRC
        );
    }

    private string[] ReadArtists(File file)
    {
        if (!(file.GetTag(TagTypes.Xiph) is XiphComment xiphTag))
        {
            return [];
        }

        string[]? artists = xiphTag.GetField(Artists);
        if (artists == null || artists.Length == 0)
        {
            return [];
        }

        return artists;
    }

    private string[] ReadStyles(File file)
    {
        if (!(file.GetTag(TagTypes.Xiph) is XiphComment xiphTag))
        {
            return [];
        }

        string[]? artists = xiphTag.GetField(Styles);
        if (artists == null || artists.Length == 0)
        {
            return [];
        }

        return artists;
    }

    // todo: write genres
    public void UpdateSongMetadata(Song song, string songPath)
    {
        using var file = File.Create(songPath);

        file.Tag.Title = song.Title;
        file.Tag.Album = song.AlbumName;
        file.Tag.Year = song.Year;
        file.Tag.AlbumArtists = [song.MainArtist];

        // WriteGenres(song, file);
        WriteStyles(song, file);
        WriteArtists(song, file);

        file.Save();
    }

    private void WriteArtists(Song song, File file)
    {
        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Artists, song.Artists.ToArray());
        }
    }

    private void WriteStyles(Song song, File file)
    {
        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            xiphTag.SetField(Styles, song.Styles.ToArray());
        }
    }

    // private void WriteGenres(Song song, File file)
    // {
    //     var genres = song.Styles.SelectMany(style => style.Genres.Select(genre => genre.Name)).ToList();
    //
    //     if (genres.Any())
    //     {
    //         file.Tag.Genres = genres.ToArray();
    //     }
    // }
}
