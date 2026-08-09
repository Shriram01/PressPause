using MediatR;

namespace PressPause.Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand(
    string Name,
    string Slug) : IRequest<Guid>;