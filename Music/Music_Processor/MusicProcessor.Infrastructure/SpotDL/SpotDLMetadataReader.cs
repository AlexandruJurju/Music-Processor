using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.SongsMetadata;
using MusicProcessor.Domain.Models.SpotDL.Parse;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLMetadataReader : ISpotDLMetadataReader
{
    private readonly IFileService _fileService;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly ILogger<SpotDLMetadataReader> _logger;

    public SpotDLMetadataReader(
        IFileService fileService,
        ILogger<SpotDLMetadataReader> logger
    )
    {
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<Dictionary<string, SongMetadata>> LoadSpotDLMetadataAsync(string playlistName)
    {
        var spotdlFile = _fileService.GetSpotDLFileInPlaylistFolder(playlistName);
        if (spotdlFile is null)
        {
            _logger.LogError("No spotdl file found in directory");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotdlFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            _logger.LogWarning("No songs found in spotdl file");
            throw new FileNotFoundException("No spotdl file found in directory");
        }

        var spotDlMetadata = new Dictionary<string, SongMetadata>();
        foreach (var spotDlSong in playlistData.Songs)
        {
            var song = spotDlSong.ToSong();
            if (!spotDlMetadata.TryAdd(song.Key, song))
            {
                _logger.LogWarning($"Duplicate song: {song.Key}");
            }
        }

        _logger.LogInformation($"Loaded metadata for {spotDlMetadata.Count} songs from spotdl file");
        return spotDlMetadata;
    }
}