using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music_Processor.CLI;
using Music_Processor.CLI.Commands;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register all commands
        services.AddTransient<ICommand, FirstTimeSyncCommand>();
        services.AddTransient<ICommand, UpdateSyncCommand>();
        services.AddTransient<ICommand, FixGenresCommand>();
        services.AddTransient<ICommand, WriteSongListCommand>();
        services.AddTransient<ICommand, WriteMetadataFileCommand>();
        services.AddTransient<ICommand, ApplyMetadataCommand>();
        services.AddTransient<ICommand, ExitCommand>();

        // Register command factory
        services.AddSingleton<CommandFactory>();

        // Register CLI
        services.AddTransient<CLI>();
    })
    .Build();

var cli = host.Services.GetRequiredService<CLI>();
await cli.RunAsync();