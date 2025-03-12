using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Interfaces.Infrastructure;
using MusicProcessor.Domain.Entities.Artits;
using MusicProcessor.Domain.Entities.Songs;
using MusicProcessor.Domain.Models.SpotDL.Parse;

namespace MusicProcessor.Infrastructure.SpotDL;

public class SpotDLMetadataLoader : ISpotDLMetadataLoader
{
    private readonly IFileService _fileService;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly ILogger<SpotDLMetadataLoader> _logger;

    public SpotDLMetadataLoader(IFileService fileService, ILogger<SpotDLMetadataLoader> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<Dictionary<string, Song>> LoadSpotDLMetadataAsync(string playlistPath)
    {
        var spotdlFile = _fileService.GetSpotDLFile(playlistPath);
        if (spotdlFile is null)
        {
            _logger.LogWarning("No spotdl file found in directory");
            return new Dictionary<string, Song>();
        }

        var playlistData = await JsonSerializer.DeserializeAsync<SpotDLPlaylist>(File.OpenRead(spotdlFile), _jsonOptions);

        if (playlistData?.Songs is not { Count: > 0 })
        {
            _logger.LogWarning("No songs found in spotdl file");
            return new Dictionary<string, Song>();
        }

        var metadataLookup = new Dictionary<string, Song>();
        foreach (var spotDlSong in playlistData.Songs)
        {
            try
            {
                var song = spotDlSong.ToSong();
                var key = CreateLookupKey(song.MainArtist, spotDlSong.Name);
                if (!metadataLookup.TryAdd(key, song))
                {
                    _logger.LogWarning("Duplicate song found and skipped: {Key}", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading song {spotDlSong.Name}: {ex.Message}");
            }
        }

        _logger.LogInformation($"Loaded metadata for {metadataLookup.Count} songs from spotdl file");
        return metadataLookup;
    }

    public string CreateLookupKey(Artist artist, string title)
    {
        var cleanedTitle = title.ToLower().Trim();

        return $"{artist.Name.ToLower().Trim()} - {cleanedTitle}";
    }
}