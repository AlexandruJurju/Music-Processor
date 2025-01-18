using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.Application.Factories;

namespace MusicProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<MetadataHandlerFactory>();

        return services;
    }
}