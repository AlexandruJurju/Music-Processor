using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.Infrastructure;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Domain.Songs;
using MusicProcessor.Infrastructure.Database;

namespace MusicProcessor.Infrastructure.MetadataService.SpotDLMetadataReader;

public class MetadataService(
    ApplicationDbContext dbContext,
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

    public async Task ExportMetadataAsync()
    {
        List<Song> songs = await dbContext.Songs
            .Include(e => e.Artists)
            .Include(e => e.Album)
            .ThenInclude(e => e.MainArtist)
            .Include(e => e.MainArtist)
            .Include(e => e.Styles)
            .ThenInclude(e => e.Genres)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();

        string json = JsonSerializer.Serialize(songs, _exportOptions);
        await File.WriteAllTextAsync(config["Paths:ExportedMetadata"]!, json);
    }
}
