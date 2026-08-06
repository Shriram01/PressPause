using MediatR;

namespace PressPause.Application.Platforms.Commands.CreatePlatform;

public sealed record CreatePlatformCommand(
    string Name,
    string Slug) : IRequest<Guid>;