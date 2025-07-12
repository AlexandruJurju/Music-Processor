using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.Console;
using MusicProcessor.Infrastructure;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.ConfigureLogging(builder.Configuration);

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

IHost host = builder.Build();

await host.StartAsync();

InteractiveCli cli = host.Services.GetRequiredService<InteractiveCli>();

await cli.RunInteractiveAsync();

await host.StopAsync();
