namespace Music_Processor.CLI.Commands;

public class ExitCommand : ICommand
{
    public string Name => "Exit";
    public int MenuNumber => MenuChoices.Exit;

    public Task ExecuteAsync()
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
