using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.Infrastructure;

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