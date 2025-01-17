using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.CLI;
using MusicProcessor.Application;
using MusicProcessor.Application.Abstractions.Interfaces;
using MusicProcessor.Application.Services;
using MusicProcessor.Infrastructure;
using MusicProcessor.Infrastructure.FileAccess;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.RegisterApplication();
        services.AddInfrastructure();
        services.RegisterCLI();
    })
    .Build();

var cli = host.Services.GetRequiredService<CommandLine>();
await cli.RunAsync();