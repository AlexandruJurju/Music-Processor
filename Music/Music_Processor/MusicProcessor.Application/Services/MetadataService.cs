using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Application;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Genres;
using MusicProcessor.Domain.SongsMetadata;
using TagLib;
using TagLib.Ogg;
using File = TagLib.File;

namespace MusicProcessor.Application.Services;

public class MetadataService : IMetadataService
{
    private const string GENRE_CATEGORY = "GENRE-CATEGORY";
    private const string ARTISTS = "ARTISTS";
    private readonly ILogger<MetadataService> _logger;

    public MetadataService(ILogger<MetadataService> logger)
    {
        _logger = logger;
    }

    public SongMetadata ReadMetadata(string songPath)
    {
        using var file = File.Create(songPath);
        SongMetadata metadata = ExtractMetadata(file, songPath);
        _logger.LogInformation("Successfully read metadata from file: {SongPath}", songPath);
        return metadata;
    }

    public void WriteMetadata(SongMetadata songMetadata, string songPath)
    {
        using var file = File.Create(songPath);
        UpdateMetadata(file, songMetadata);
        file.Save();
        _logger.LogInformation("Successfully updated metadata for file: {SongPath}", songPath);
    }

    private SongMetadata ExtractMetadata(File file, string songPath)
    {
        Tag? tag = file.Tag;

        var metadata = new SongMetadata(
            tag.Title ?? Path.GetFileNameWithoutExtension(songPath),
            tag.ISRC,
            ReadCustomTagArtists(file),
            new Artist(tag.FirstPerformer ?? string.Empty),
            ReadGenres(file),
            (int)tag.Disc,
            (int)tag.DiscCount,
            tag.Album != null
                ? new Album(tag.Album, new Artist(tag.FirstAlbumArtist ?? string.Empty))
                : null,
            file.Properties.Duration.Seconds,
            (int)tag.Year,
            (int)tag.Track,
            (int)tag.TrackCount
        );

        return metadata;
    }

    private List<Genre> ReadGenres(File file)
    {
        _logger.LogInformation("Extracting genres from file {FileName}", file.Name);
        var genres = new List<string>();

        if (file.TagTypes.HasFlag(TagTypes.Xiph))
        {
            var xiphTag = file.GetTag(TagTypes.Xiph) as XiphComment;
            if (xiphTag != null)
            {
                string[]? genreFields = xiphTag.GetField("GENRE");
                genres.AddRange(genreFields);
            }
            else
            {
                _logger.LogWarning("No Xiph comment tag found in file");
            }
        }
        else
        {
            _logger.LogWarning("No Xiph tags found in file");
        }

        return genres.Distinct()
            .Select(genreName => new Genre(genreName))
            .ToList();
    }

    private List<Artist> ReadCustomTagArtists(File file)
    {
        if (!(file.GetTag(TagTypes.Xiph) is XiphComment xiphTag))
        {
            _logger.LogWarning("No Xiph comment tag found in file {FileName}", file.Name);
            return new List<Artist>();
        }

        string[]? artists = xiphTag.GetField(ARTISTS);
        if (artists == null || artists.Length == 0)
        {
            return new List<Artist>();
        }

        return artists
            .Select(a => new Artist(a))
            .ToList();
    }

    private void UpdateMetadata(File file, SongMetadata songMetadata)
    {
        Tag? tag = file.Tag;
        tag.Title = songMetadata.Name;
        tag.Album = songMetadata.Album?.Name;
        tag.Year = (uint)songMetadata.Date;
        tag.AlbumArtists = new[] { songMetadata.MainArtist.Name };
        UpdateAdditionalMetadata(file, songMetadata);
    }

    private void UpdateAdditionalMetadata(File file, SongMetadata songMetadata)
    {
        WriteCustomTagGenreCategories(file, songMetadata);
        WriteCustomTagArtists(file, songMetadata);
        WriteGenres(file, songMetadata);
    }

    private void WriteGenres(File file, SongMetadata songMetadata)
    {
        string[] genres = songMetadata.Genres
            .Where(g => !g.RemoveFromSongs)
            .Select(g => g.Name)
            .ToArray();

        if (genres.Any())
        {
            _logger.LogInformation("Setting genres: {Genres}", string.Join(", ", genres));
            file.Tag.Genres = genres;
        }
    }

    private void WriteCustomTagGenreCategories(File file, SongMetadata songMetadata)
    {
        string[] genreCategories = songMetadata.Genres
            .SelectMany(g => g.GenreCategories.Select(c => c.Name))
            .Distinct()
            .ToArray();

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            _logger.LogInformation("Setting genre categories: {GenreCategories}", string.Join(", ", genreCategories));
            xiphTag.SetField(GENRE_CATEGORY, genreCategories);
        }
    }

    private void WriteCustomTagArtists(File file, SongMetadata songMetadata)
    {
        string[] artists = songMetadata.Artists
            .Select(a => a.Name)
            .ToArray();

        if (file.GetTag(TagTypes.Xiph) is XiphComment xiphTag)
        {
            _logger.LogInformation("Setting artists: {Artists}", string.Join(", ", artists));
            xiphTag.SetField(ARTISTS, artists);
        }
    }
}
