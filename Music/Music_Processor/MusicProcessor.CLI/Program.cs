using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Configurations;
using MusicProcessor.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureLogging();
builder.Services
    .RegisterApplication()
    .RegisterInfrastructure(builder.Configuration)
    .RegisterCLI();

var host = builder.Build();

var cli = host.Services.GetRequiredService<InteractiveCli>();
await cli.RunInteractiveAsync();