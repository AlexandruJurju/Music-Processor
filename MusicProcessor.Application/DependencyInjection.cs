using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Abstractions.Behaviors;
using MusicProcessor.Domain.Abstractions;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        AddMediator(services);
        
        return services;
    }

    private static void AddMediator(IServiceCollection services)
    {
        services.AddMediator(options =>
            {
                options.Assemblies = [typeof(DependencyInjection), typeof(IDomainEvent)];
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.NotificationPublisherType = typeof(ForeachAwaitPublisher);
            }
        );
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }
}
