﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music_Processor;
using Music_Processor.CLI;
using Music_Processor.CLI.Commands;
using Music_Processor.Factories;
using Music_Processor.Interfaces;
using Music_Processor.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register services
        services.AddTransient<ISpotDLService, SpotDLService>();
        services.AddTransient<IFileService, FileService>();

        // Register all commands
        services.AddTransient<IMenuCommand, FirstTimeSyncCommand>();
        services.AddTransient<IMenuCommand, UpdateSyncCommand>();
        services.AddTransient<IMenuCommand, FixGenresCommand>();
        services.AddTransient<IMenuCommand, WriteSongListCommand>();
        services.AddTransient<IMenuCommand, WriteMetadataFileCommand>();
        services.AddTransient<IMenuCommand, ApplyMetadataCommand>();
        services.AddTransient<IMenuCommand, ExitCommand>();

        services.AddTransient<IConfigService, ConfigService>();
        services.AddTransient<IMetadataService, JsonMetadataService>();

        // Register command factory
        services.AddSingleton<MenuCommandFactory>();
        services.AddSingleton<MetadataExtractorFactory>();

        // Register CLI
        services.AddTransient<CLI>();
    })
    .Build();


// var cli = host.Services.GetRequiredService<CLI>();
// await cli.RunAsync();