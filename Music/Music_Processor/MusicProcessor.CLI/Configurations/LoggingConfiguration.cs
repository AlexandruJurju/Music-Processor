﻿using Microsoft.Extensions.Hosting;
using MusicProcessor.Application.UseCases.FixMetadata;
using MusicProcessor.Application.UseCases.WriteLibraryWithSpotdlFile;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MusicProcessor.CLI.Configurations;

public static class LoggingConfiguration
{
    public static IHostApplicationBuilder ConfigureLogging(this IHostApplicationBuilder builder)
    {
        Log.Logger = CreateLogger();
        builder.Services.AddSerilog();
        return builder;
    }

    private static ILogger CreateLogger()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var configuration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
                theme: AnsiConsoleTheme.Literate);

        AddHandlerLogger(configuration, nameof(WriteLibraryWithSpotdlFileCommandHandler), LogEventLevel.Warning, timestamp);
        AddHandlerLogger(configuration, nameof(FixMetadataCommandHandler), LogEventLevel.Information, timestamp);

        return configuration.CreateLogger();
    }

    private static void AddHandlerLogger(LoggerConfiguration configuration, string handlerName, LogEventLevel logLevel, string timestamp)
    {
        configuration.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(evt =>
                evt.Level == logLevel &&
                evt.Properties.ContainsKey("SourceContext") &&
                evt.Properties["SourceContext"].ToString().Contains(handlerName))
            .WriteTo.File(
                $"logs/{timestamp}_{handlerName}.log",
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message:lj}{NewLine}",
                shared: false));
    }
}