using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.SpotDL.Interfaces;
using MusicProcessor.SpotDL.Models;

namespace MusicProcessor.SpotDL.SpotDL;

public class SpotDLMetadataReader : ISpotDLMetadataReader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly ILogger<SpotDLMetadataReader> _logger;

    public SpotDLMetadataReader(ILogger<SpotDLMetadataReader> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, SpotDLSongMetadata>> LoadSpotDLMetadataAsync(string spotDLFile)
    {
        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotDLFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            _logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var spotDlMetadata = new Dictionary<string, SpotDLSongMetadata>();
        foreach (var spotDlSong in playlistData.Songs)
        {
            if (!spotDlMetadata.TryAdd(spotDlSong.Key, spotDlSong))
            {
                _logger.LogWarning($"Duplicate song: {spotDlSong.Key}");
            }
        }

        _logger.LogInformation($"Loaded metadata for {spotDlMetadata.Count} songs from spotdl file");
        return spotDlMetadata;
    }
}