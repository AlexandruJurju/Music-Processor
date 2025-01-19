using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicProcessor.Application;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using MusicProcessor.CLI;
using MusicProcessor.CLI.Extensions;
using MusicProcessor.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
        theme: AnsiConsoleTheme.Code)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt =>
            evt.Level == LogEventLevel.Warning &&
            evt.Properties.ContainsKey("SourceContext") &&
            evt.Properties["SourceContext"].ToString().Contains(nameof(WriteLibraryWithSpotdlFileCommandHandler)))
        .WriteTo.File("logs/spotdl-warnings.log",
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj}{NewLine}",
            shared: false))
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddSerilog();

builder.Services.RegisterApplication();
builder.Services.AddInfrastructure();
builder.Services.RegisterCLI();

var host = builder.Build();

var cli = host.Services.GetRequiredService<CommandLine>();
await cli.RunAsync();