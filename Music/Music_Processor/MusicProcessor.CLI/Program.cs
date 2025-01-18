using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Extensions;
using MusicProcessor.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterApplication();
builder.Services.AddInfrastructure();
builder.Services.RegisterCLI();

// Seed the database
await builder.SeedDataAsync();

var host = builder.Build();

var cli = host.Services.GetRequiredService<CommandLine>();
await cli.RunAsync();