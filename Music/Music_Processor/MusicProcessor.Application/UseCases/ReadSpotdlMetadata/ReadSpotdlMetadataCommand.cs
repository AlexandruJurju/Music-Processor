using MediatR;

namespace MusicProcessor.Application.UseCases.ReadSpotdlMetadata;

public record ReadSpotdlMetadataCommand(string PlaylistName) : IRequest;
