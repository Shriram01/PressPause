using MediatR;

namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record GetGameByIdQuery(Guid Id)
    : IRequest<GameDetailsDto?>;