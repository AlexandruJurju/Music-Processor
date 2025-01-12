namespace Music_Processor.CLI.Commands;

public interface IMenuCommand
{
    string Name { get; }
    int MenuNumber { get; }
    Task ExecuteAsync();
}