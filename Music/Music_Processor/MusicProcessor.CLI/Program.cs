using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Configurations;
using MusicProcessor.Infrastructure;
using MusicProcessor.SpotDL;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.ConfigureLogging(builder.Configuration);

builder.Services
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration)
    .RegisterCli();

builder.Services.RegisterSpotDL();

IHost host = builder.Build();

await host.StartAsync();

InteractiveCli cli = host.Services.GetRequiredService<InteractiveCli>();
await cli.RunInteractiveAsync();

await host.StopAsync();
