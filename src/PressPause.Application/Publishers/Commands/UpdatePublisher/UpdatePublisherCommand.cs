using MediatR;

namespace PressPause.Application.Publishers.Commands.UpdatePublisher;

public sealed record UpdatePublisherCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;