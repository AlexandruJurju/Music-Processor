using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Albums;
using MusicProcessor.Domain.Artists;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Domain.Styles;

namespace MusicProcessor.Infrastructure.SpotDLMetadataReader;

public class SpotDLMetadataReader(
    IConfiguration config,
    ILogger<SpotDLMetadataReader> logger
) : ISpotDLMetadataReader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<List<Song>> LoadSpotDLMetadataAsync()
    {
        await using FileStream fileStream = File.OpenRead(config["Paths:MetadataFile"]!);
        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(fileStream, _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var artistCache = new Dictionary<string, Artist>();
        var albumCache = new Dictionary<string, Album>();
        var styleCache = new Dictionary<string, Style>();

        foreach (SpotDLSongMetadata song in playlistData.Songs)
        {
            GetOrCreateArtist(song.Artist, artistCache);

            GetOrCreateArtist(song.AlbumArtist, artistCache);

            foreach (string artistName in song.Artists)
            {
                GetOrCreateArtist(artistName, artistCache);
            }
        }

        foreach (SpotDLSongMetadata song in playlistData.Songs)
        {
            Artist albumArtist = GetOrCreateArtist(song.AlbumArtist, artistCache);
            GetOrCreateAlbum(song.AlbumName, albumArtist, albumCache);
        }

        var loadedSpotdlMetadata = playlistData.Songs.Select(song =>
        {
            Artist mainArtist = artistCache[song.Artist.ToUpperInvariant()];
            Artist albumArtist = artistCache[song.AlbumArtist.ToUpperInvariant()];
            Album album = GetOrCreateAlbum(song.AlbumName, albumArtist, albumCache);

            var artists = song.Artists
                .Select(artistName => artistCache[artistName.ToUpperInvariant()])
                .ToList();

            var styles = song.Genres
                .Select(genre => GetOrCreateStyle(genre, styleCache))
                .ToList();

            return song.ToSong(mainArtist, artists, album, styles);
        }).ToList();
        
        return loadedSpotdlMetadata;
    }


    private Artist GetOrCreateArtist(string name, Dictionary<string, Artist> cache)
    {
        string key = name.ToUpperInvariant();
        if (!cache.TryGetValue(key, out Artist? artist))
        {
            artist = Artist.Create(name);
            cache[key] = artist;
        }

        return artist;
    }

    private Album GetOrCreateAlbum(string name, Artist albumArtist, Dictionary<string, Album> cache)
    {
        string key = $"{name.ToUpperInvariant()}|{albumArtist.Name.ToUpperInvariant()}";
        if (!cache.TryGetValue(key, out Album? album))
        {
            album = Album.Create(name, albumArtist);
            cache[key] = album;
        }

        return album;
    }

    private Style GetOrCreateStyle(string genre, Dictionary<string, Style> cache)
    {
        string key = genre.ToUpperInvariant();
        if (!cache.TryGetValue(key, out Style? style))
        {
            style = Style.Create(genre, false);
            cache[key] = style;
        }

        return style;
    }
}
