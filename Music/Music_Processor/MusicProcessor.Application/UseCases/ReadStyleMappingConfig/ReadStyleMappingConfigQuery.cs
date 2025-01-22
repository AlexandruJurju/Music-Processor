using MediatR;
using MusicProcessor.Domain.Entities;

namespace MusicProcessor.Application.UseCases.ReadStyleMappingConfig;

public sealed record GetStyleMappingConfigQuery : IRequest<IEnumerable<Style>>;