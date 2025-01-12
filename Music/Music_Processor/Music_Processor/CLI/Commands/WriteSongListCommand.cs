namespace Music_Processor.CLI.Commands;

public class WriteSongListCommand : IMenuCommand
{
    public string Name => "Write song list file";
    public int MenuNumber => MenuChoices.WriteSongList;

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}