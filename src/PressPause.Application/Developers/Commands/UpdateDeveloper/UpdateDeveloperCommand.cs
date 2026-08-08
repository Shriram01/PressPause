using MediatR;

namespace PressPause.Application.Developers.Commands.UpdateDeveloper;

public sealed record UpdateDeveloperCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;