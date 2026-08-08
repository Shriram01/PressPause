using MediatR;

namespace PressPause.Application.Publishers.Commands.DeletePublisher;

public sealed record DeletePublisherCommand(Guid Id) : IRequest<bool>;