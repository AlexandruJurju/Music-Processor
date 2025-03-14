using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Songs;
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

    public async Task<IEnumerable<Song>> LoadSpotDLMetadataAsync(string playlistName)
    {
        var spotdlFile = _fileService.GetSpotDLFileInPlaylistFolder(playlistName);
        if (spotdlFile is null)
        {
            _logger.LogWarning("No spotdl file found in directory");
            return new List<Song>();
        }

        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotdlFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            _logger.LogWarning("No songs found in spotdl file");
            return new List<Song>();
        }

        var spotDlMetadata = new List<Song>();
        foreach (var spotDlSong in playlistData.Songs)
        {
            try
            {
                var song = spotDlSong.ToSong();
                spotDlMetadata.Add(song);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading song {spotDlSong.Name}: {ex.Message}");
            }
        }

        _logger.LogInformation($"Loaded metadata for {spotDlMetadata.Count} songs from spotdl file");
        return spotDlMetadata;
    }
}