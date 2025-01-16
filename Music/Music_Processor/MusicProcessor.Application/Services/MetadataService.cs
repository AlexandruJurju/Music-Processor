using System.Text.Json;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Factories;
using MusicProcessor.Domain.Constants;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.Services;

public class MetadataService : IMetadataService
{
    private readonly ILogger<MetadataService> _logger;
    private readonly MetadataHandlerFactory _metadataHandlerFactory;

    public MetadataService(ILogger<MetadataService> logger)
    {
        _metadataHandlerFactory = new MetadataHandlerFactory();
        _logger = logger;
    }

    public List<Song> GetPlaylistSongsMetadata(string directoryPath, bool recursive = false)
    {
        var metadata = new List<Song>();
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


    public void WriteSongMetadata(string songPath, Song song)
    {
        var handle = _metadataHandlerFactory.GetHandler(songPath);
        handle.WriteMetadata(songPath, song);
    }

    public Song ExtractMetadataFromSong(string songPath)
    {
        var handle = _metadataHandlerFactory.GetHandler(songPath);
        return handle.ExtractMetadata(songPath);
    }
}