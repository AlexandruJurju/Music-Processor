using Microsoft.Extensions.Logging;

namespace Music_Processor.CLI.Commands;

public class ApplyMetadataCommand : IMenuCommand
{
    private readonly ILogger<ApplyMetadataCommand> _logger;

    public ApplyMetadataCommand(ILogger<ApplyMetadataCommand> logger)
    {
        _logger = logger;
    }

    public string Name => "Apply Metadata from json";

    public Task ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}