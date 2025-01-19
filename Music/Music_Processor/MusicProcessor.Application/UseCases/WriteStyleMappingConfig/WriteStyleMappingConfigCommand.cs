using MediatR;

namespace MusicProcessor.Application.UseCases.WriteStyleMappingConfig;

public sealed record WriteStyleMappingConfigCommand() : IRequest;