using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Model;

namespace MusicProcessor.Application.Services;

public class MetadataService : IMetadataService
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<MetadataService> _logger;
    private readonly MetadataHandlerFactory _metadataHandlerFactory;
    private readonly IMetadataSerializationStrategy _metadataSerializationStrategy;

    public MetadataService(ILogger<MetadataService> logger, IMetadataSerializationStrategy metadataSerializationStrategy)
    {
        _metadataHandlerFactory = new MetadataHandlerFactory();
        _logger = logger;
        _metadataSerializationStrategy = metadataSerializationStrategy;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public List<AudioMetadata> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false)
    {
        var metadata = new List<AudioMetadata>();
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var files = Directory.GetFiles(directoryPath, "*.*", searchOption)
            .Where(f => Constants.ProcessableAudioFileFormats.Contains(Path.GetExtension(f).ToLower()));

        foreach (var file in files)
        {
            try
            {
                var extractor = _metadataHandlerFactory.GetHandler(file);
                var audioMetadata = extractor.ExtractMetadata(file);
                metadata.Add(audioMetadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file: {FilePath}", file);
            }
        }

        return metadata;
    }

    public async Task SaveMetadataToFileAsync(List<AudioMetadata> metadata, string playlistName)
    {
        await _metadataSerializationStrategy.SaveMetadataAsync(metadata, playlistName);
    }

    public async Task<List<AudioMetadata>> LoadMetadataFromFileAsync(string playlistName)
    {
        return await _metadataSerializationStrategy.LoadMetadataAsync(playlistName);
    }

    public void WriteSongMetadata(string songPath, AudioMetadata audioMetadata)
    {
        var handle = _metadataHandlerFactory.GetHandler(songPath);
        handle.WriteMetadata(songPath, audioMetadata);
    }

    public AudioMetadata ExtractMetadataFromSong(string songPath)
    {
        var handle = _metadataHandlerFactory.GetHandler(songPath);
        return handle.ExtractMetadata(songPath);
    }
}