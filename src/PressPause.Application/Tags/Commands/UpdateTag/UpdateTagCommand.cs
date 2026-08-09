using MediatR;

namespace PressPause.Application.Tags.Commands.UpdateTag;

public sealed record UpdateTagCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;