using MediatR;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application.Abstractions.DataAccess;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.ReadStyleMappingConfig;

public class ReadStyleMappingConfigHandler(
    IConfigRepository configRepository,
    ILogger<ReadStyleMappingConfigHandler> logger
) : IRequestHandler<GetStyleMappingConfigQuery, IEnumerable<Style>>
{
    public async Task<IEnumerable<Style>> Handle(GetStyleMappingConfigQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Message}", "Starting to read style mapping configuration");
        try
        {
            var styles = await configRepository.ReadStyleMappingAsync();
            logger.LogInformation("{Message}", "Successfully read style mapping configuration");
            return styles;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Message}", "Error reading style mapping configuration");
            throw;
        }
    }
}