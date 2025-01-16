namespace MusicProcessor.CLI.MenuCommands;

public class ExitCommand : IMenuCommand
{
    public string Name => "Exit";

    public Task ExecuteAsync()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}