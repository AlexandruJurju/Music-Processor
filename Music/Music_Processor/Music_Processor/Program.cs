using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Music_Processor.CLI;

var services = new ServiceCollection();
services.AddLogging(builder => 
{
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddSingleton<CLI>();

var serviceProvider = services.BuildServiceProvider();
var cli = serviceProvider.GetRequiredService<CLI>();

await cli.RunAsync();