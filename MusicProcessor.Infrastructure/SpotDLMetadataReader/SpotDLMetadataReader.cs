using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.SpotDLMetadataReader;

public class SpotDLMetadataReader(
    ILogger<SpotDLMetadataReader> logger
) : ISpotDLMetadataReader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string path)
    {
        await using FileStream fileStream = File.OpenRead(path);

        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(fileStream, _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var spotDlMetadata = new Dictionary<string, Song>();

        foreach (SpotDLSongMetadata spotDlSong in playlistData.Songs)
        {
            if (spotDlMetadata.TryGetValue(spotDlSong.Key, out Song value))
            {
                logger.LogWarning("Replacing duplicate song key: {Key} (Old: {Title}, New: {NewName})", spotDlSong.Key, value.Title, spotDlSong.Name);
            }

            spotDlMetadata[spotDlSong.Key] = spotDlSong.ToSong();
        }

        logger.LogInformation("Loaded metadata for {Count} songs from spotdl file", spotDlMetadata.Count);
        return spotDlMetadata;
    }
}
