using System.Text.Json;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Constants;

namespace MusicProcessor.Infrastructure.FileAccess;

public class ConfigService : IConfigService
{
    private readonly IFileService _fileService;

    public ConfigService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Dictionary<string, List<string>> LoadStyleMappingFile()
    {
        try
        {
            using var r = new StreamReader(Path.Combine(_fileService.GetDataAccessFolderPath(), Constants.StyleMappingFile));
            Dictionary<string, List<string>> styleMapping = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(r.ReadToEnd(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            if (styleMapping == null)
            {
                throw new InvalidOperationException("Failed to deserialize style mapping file");
            }

            return styleMapping;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Style mapping file not found at expected location: {Constants.StyleMappingFile}");
            return Constants.DEFAULT_STYLE_MAPPINGS;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format in style mapping file", ex);
        }
    }

    public List<string> LoadStylesToRemove()
    {
        try
        {
            using var r = new StreamReader(Path.Combine(_fileService.GetDataAccessFolderPath(), Constants.StylesToRemoveFile));
            List<string> stylesToRemove = JsonSerializer.Deserialize<List<string>>(r.ReadToEnd(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return stylesToRemove;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Styles to remove file not found at expected location: {Constants.StylesToRemoveFile}");
            return Constants.DEFAULT_STYLES_TO_REMOVE;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format in style mapping file", ex);
        }
    }
}