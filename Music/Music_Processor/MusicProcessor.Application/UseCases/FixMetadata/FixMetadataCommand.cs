using MediatR;

namespace MusicProcessor.Application.UseCases.FixMetadata;

public record FixMetadataCommand(string playlistPath) : IRequest;