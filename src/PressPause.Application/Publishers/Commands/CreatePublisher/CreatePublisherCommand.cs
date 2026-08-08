using MediatR;

namespace PressPause.Application.Publishers.Commands.CreatePublisher;

public sealed record CreatePublisherCommand(
    string Name,
    string Slug) : IRequest<Guid>;