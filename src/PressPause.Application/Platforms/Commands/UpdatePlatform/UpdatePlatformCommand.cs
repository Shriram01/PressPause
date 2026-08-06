using MediatR;

namespace PressPause.Application.Platforms.Commands.UpdatePlatform;

public sealed record UpdatePlatformCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;