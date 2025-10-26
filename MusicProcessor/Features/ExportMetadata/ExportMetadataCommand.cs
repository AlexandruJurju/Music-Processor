using Spectre.Console.Cli;

namespace MusicProcessor.Features.ExportMetadata;

internal sealed class ExportMetadataCommand : AsyncCommand<ExportMetadataCommand.Settings>
{
    
    internal sealed class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}