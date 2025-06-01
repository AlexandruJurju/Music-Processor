using System.Diagnostics;
using Mediator;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;

namespace MusicProcessor.WebApi.Endpoints;

public class Test : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("test", async (ISender sender) =>
            {
                // var logMissing = new LogMissingCommand();
                // await sender.Send(logMissing);

                var stopwatch = Stopwatch.StartNew();

                var readMetadataCommand = new ReadMetadataFromFileCommand();
                await sender.Send(readMetadataCommand);

                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);

                // var command2 = new FixSongMetadataCommand();
                // await sender.Send(command2);

                // var command3 = new TestCommand();
                // await sender.Send(command3);
            })
            .WithTags("test");
    }
}
