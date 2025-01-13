using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music_Processor.CLI;
using Music_Processor.CLI.Commands;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Services;
using Music_Processor.Services.SpotDLMetadataLoader;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddTransient<ISpotDLService, SpotDLService>();
        services.AddTransient<IFileService, FileService>();

        // Register all commands
        services.AddTransient<IMenuCommand, FirstTimeSyncCommand>();
        services.AddTransient<IMenuCommand, UpdateSyncCommand>();
        services.AddTransient<IMenuCommand, FixGenresSpotDLCommand>();
        services.AddTransient<IMenuCommand, WriteMetadataFileCommand>();
        services.AddTransient<IMenuCommand, FixGenresCustomMetadata>();
        services.AddTransient<IMenuCommand, ExitCommand>();

        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IMetadataService, JsonMetadataService>();
        services.AddTransient<IPlaylistProcessor, PlaylistProcessor>();

        // Register command factory
        services.AddSingleton<MenuCommandFactory>();
        services.AddSingleton<MetadataHandlesFactory>();

        services.AddSingleton<SpotdlMetadataLoader>();

        // Register CLI
        services.AddTransient<CLI>();
    })
    .Build();

var cli = host.Services.GetRequiredService<CLI>();
await cli.RunAsync();
