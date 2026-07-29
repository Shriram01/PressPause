using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Games.Queries.GetGames;

public sealed record GetGamesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null)
    : IRequest<PagedResult<GameDto>>;