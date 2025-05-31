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

        return playlistData.Songs.Select(song =>
        {
            var mainArtist = Artist.Create(song.Artist);
            var albumArtist = Artist.Create(song.AlbumArtist);
            var album = Album.Create(song.AlbumName, albumArtist);

            var artists = song.Artists
                .Select(Artist.Create)
                .ToList();

            var styles = song.Genres
                .Select(genre => Style.Create(genre, false))
                .ToList();

            return song.ToSong(mainArtist, artists, album, styles);
        }).ToList();
    }
}
