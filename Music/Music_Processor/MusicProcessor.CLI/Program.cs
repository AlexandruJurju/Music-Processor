using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Configurations;
using MusicProcessor.Infrastructure;
using MusicProcessor.Persistence;
using MusicProcessor.SpotDL;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .RegisterApplication()
    .RegisterPersistence(builder.Configuration)
    .RegisterInfrastructure(builder.Configuration)
    .RegisterCLI();

builder.Services.RegisterSpotDL();

var host = builder.Build();

var cli = host.Services.GetRequiredService<InteractiveCli>();
await cli.RunInteractiveAsync();