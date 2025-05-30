using Mediator;
using MusicProcessor.Application.Songs.FixSongMetadata;
using MusicProcessor.Application.Songs.ReadMetadataFromFile;
using MusicProcessor.Application.Songs.Test;

namespace MusicProcessor.WebApi.Endpoints;

public class Test : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("test", async (ISender sender) =>
            {
                var readMetadataCommand = new ReadMetadataFromFileCommand();
                await sender.Send(readMetadataCommand);
                
                var command2 = new FixSongMetadataCommand();
                await sender.Send(command2);

                // var command3 = new TestCommand();
                // await sender.Send(command3);
            })
            .WithTags("test");
    }
}
