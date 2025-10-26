using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicProcessor.CLI;
using MusicProcessor.Features.ExportMetadata;
using Spectre.Console.Cli;

ServiceCollection registrations = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

registrations.AddSingleton<IConfiguration>(configuration);

var registrar = new MyTypeRegistrar(registrations);

var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.AddCommand<ExportMetadataCommand>("export-metadata");
});

return app.Run(args);
