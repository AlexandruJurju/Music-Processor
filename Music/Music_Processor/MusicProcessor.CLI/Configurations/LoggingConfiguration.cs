using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MusicProcessor.CLI.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        Log.Logger = CreateLogger(configuration);
        builder.Services.AddSerilog();
    }

    private static ILogger CreateLogger(IConfiguration configuration)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        string? logPath = configuration.GetValue<string>("PathsSettings:LogsPath");

        LoggerConfiguration loggingConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}",
                restrictedToMinimumLevel: LogEventLevel.Warning,
                theme: AnsiConsoleTheme.Literate)
            .WriteTo.File(
                Path.Combine($"{logPath}\\{timestamp}.log"),
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Level:u3} {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollOnFileSizeLimit: false);

        return loggingConfig.CreateLogger();
    }
}
