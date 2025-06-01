using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;

namespace MusicProcessor.Infrastructure.MetadataService.SpotDLMetadataReader;

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

    public async Task<List<SpotDLSongMetadata>> LoadSpotDLMetadataAsync()
    {
        await using FileStream fileStream = File.OpenRead(config["Paths:MetadataFile"]!);
        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(fileStream, _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        return playlistData.Songs;
    }
}
