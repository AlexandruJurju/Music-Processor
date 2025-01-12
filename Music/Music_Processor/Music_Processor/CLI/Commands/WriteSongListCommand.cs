namespace Music_Processor.CLI.Commands;

public class WriteSongListCommand : ICommand
{
    public string Name => "Write song list file";
    public int MenuNumber => MenuChoices.WriteSongList;

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}