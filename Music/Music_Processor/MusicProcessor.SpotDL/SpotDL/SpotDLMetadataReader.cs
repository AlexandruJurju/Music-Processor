using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.SpotDL.Interfaces;
using MusicProcessor.SpotDL.Models;

namespace MusicProcessor.SpotDL.SpotDL;

public class SpotDLMetadataReader(ILogger<SpotDLMetadataReader> logger) : ISpotDLMetadataReader
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<Dictionary<string, SpotDLSongMetadata>> LoadSpotDLMetadataAsync(string spotDLFile)
    {
        // Read and deserialize the playlist file
        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotDLFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var spotDlMetadata = new Dictionary<string, SpotDLSongMetadata>();

        foreach (SpotDLSongMetadata spotDlSong in playlistData.Songs)
        {
            if (spotDlMetadata.TryGetValue(spotDlSong.Key, out SpotDLSongMetadata? value))
            {
                logger.LogWarning("Replacing duplicate song key: {Key} (Old: {ValueName}, New: {Name})", spotDlSong.Key, value.Name, spotDlSong.Name);
            }

            // This will add new or replace existing entry
            spotDlMetadata[spotDlSong.Key] = spotDlSong;
        }

        logger.LogInformation("Loaded metadata for {Count} songs from spotdl file", spotDlMetadata.Count);
        return spotDlMetadata;
    }
}
