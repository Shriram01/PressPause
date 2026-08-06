using MediatR;

namespace PressPause.Application.Platforms.Commands.DeletePlatform;

public sealed record DeletePlatformCommand(Guid Id) : IRequest<bool>;