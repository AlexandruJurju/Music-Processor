using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain.Songs;

namespace MusicProcessor.Infrastructure.MetadataService.MetadataService;

public class MetadataService(
    IConfiguration config,
    ILogger<MetadataService> logger
) : IMetadataService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly JsonSerializerOptions _exportOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task<List<SpotDLSongMetadata>> LoadSpotDlMetadataAsync()
    {
        await using FileStream fileStream = File.OpenRead(config["Paths:SpotdlMetadataFile"]!);
        SpotDLPlaylist? playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(fileStream, _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        return playlistData.Songs;
    }

    public async Task ExportMetadataAsync(IEnumerable<Song> songs, string path)
    {
        string json = JsonSerializer.Serialize(songs, _exportOptions);
        await File.WriteAllTextAsync(path, json);
    }
}
