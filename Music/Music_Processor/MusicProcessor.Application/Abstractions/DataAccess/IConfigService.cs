namespace MusicProcessor.Application.Abstractions.DataAccess;

public interface IConfigService
{
    Dictionary<string, List<string>> LoadStyleMappingFile();
    List<string> LoadStylesToRemove();
}