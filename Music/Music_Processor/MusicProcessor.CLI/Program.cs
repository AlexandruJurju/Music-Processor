using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.CLI;
using MusicProcessor.Application;
using MusicProcessor.Infrastructure;
using MusicProcessor.Domain.Constants;

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