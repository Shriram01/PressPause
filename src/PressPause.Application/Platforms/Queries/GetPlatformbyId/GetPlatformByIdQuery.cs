using MediatR;
using PressPause.Application.Platforms.Queries.GetPlatforms;

namespace PressPause.Application.Platforms.Queries.GetPlatformById;

public sealed record GetPlatformByIdQuery(Guid Id)
    : IRequest<PlatformDto?>;