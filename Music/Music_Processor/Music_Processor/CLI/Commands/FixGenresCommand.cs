using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI.Commands;

public class FixGenresCommand : ICommand
{
    private readonly ILogger<FixGenresCommand> _logger;

    public FixGenresCommand(ILogger<FixGenresCommand> logger)
    {
        _logger = logger;
    }

    public string Name => "Fix Genres";
    public int MenuNumber => MenuChoices.FixGenres;

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}