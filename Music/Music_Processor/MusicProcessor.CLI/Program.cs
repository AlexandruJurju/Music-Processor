using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicProcessor.Application;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Configurations;
using MusicProcessor.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureLogging();
builder.Services.RegisterApplication();
builder.Services.AddInfrastructure();
builder.Services.RegisterCLI();

var host = builder.Build();

var cli = host.Services.GetRequiredService<CommandLineApp>();

while(true)
{
    var newArgs = Console.ReadLine()?.Split(' ') ?? [];
    await cli.RunAsync(newArgs);
}