﻿using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MusicProcessor.CLI.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this IHostApplicationBuilder builder)
    {
        Log.Logger = CreateLogger();
        builder.Services.AddSerilog();
    }

    private static ILogger CreateLogger()
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var configuration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
                theme: AnsiConsoleTheme.Literate)
            .WriteTo.File(
                $"logs/{timestamp}_all_logs.log", // Log everything to this file
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}",
                shared: false);


        return configuration.CreateLogger();
    }

    private static void AddCQRSHandlerLogger(LoggerConfiguration configuration, string handlerName, LogEventLevel logLevel, string timestamp)
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