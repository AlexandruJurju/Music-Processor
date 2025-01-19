using MusicProcessor.CLI.MenuOptions.Abstractions;

namespace MusicProcessor.CLI.MenuOptions;

public class ExitOption : IMenuOption
{
    public string Name => "Exit";

    public Task ExecuteAsync()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}