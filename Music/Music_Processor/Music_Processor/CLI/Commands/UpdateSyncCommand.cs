using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI.Commands;

public class UpdateSyncCommand : ICommand
{
    private readonly ILogger<UpdateSyncCommand> _logger;

    public UpdateSyncCommand(ILogger<UpdateSyncCommand> logger)
    {
        _logger = logger;
    }

    public string Name => "Update Sync";
    public int MenuNumber => MenuChoices.UpdateSync;

    public async Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}