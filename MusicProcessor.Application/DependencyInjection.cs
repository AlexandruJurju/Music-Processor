using MusicProcessor.Domain.Abstractions;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Behaviors;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddMediator(options =>
            {
                options.Assemblies = [typeof(DependencyInjection), typeof(IDomainEvent)];
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.NotificationPublisherType = typeof(ForeachAwaitPublisher);
            }
        );
        return services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }
}
