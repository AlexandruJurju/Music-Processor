using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI.Commands;

public class ApplyMetadataCommand : ICommand
{
    private readonly ILogger<ApplyMetadataCommand> _logger;

    public ApplyMetadataCommand(ILogger<ApplyMetadataCommand> logger)
    {
        _logger = logger;
    }

    public string Name => "Apply Metadata from json";
    public int MenuNumber => MenuChoices.ApplyMetadata;

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}