using MediatR;

namespace PressPause.Application.Developers.Commands.DeleteDeveloper;

public sealed record DeleteDeveloperCommand(Guid Id) : IRequest<bool>;