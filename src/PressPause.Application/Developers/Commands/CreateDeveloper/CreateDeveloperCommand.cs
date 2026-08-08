using MediatR;

namespace PressPause.Application.Developers.Commands.CreateDeveloper;

public sealed record CreateDeveloperCommand(
    string Name,
    string Slug) : IRequest<Guid>;