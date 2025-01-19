namespace MusicProcessor.CLI.MenuOptions.Abstractions;

public interface IMenuOption
{
    string Name { get; }
    Task ExecuteAsync();
}