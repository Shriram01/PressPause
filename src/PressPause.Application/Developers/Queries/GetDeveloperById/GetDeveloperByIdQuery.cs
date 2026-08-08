using MediatR;
using PressPause.Application.Developers.Queries.GetDevelopers;

namespace PressPause.Application.Developers.Queries.GetDeveloperById;

public sealed record GetDeveloperByIdQuery(Guid Id)
    : IRequest<DeveloperDto?>;