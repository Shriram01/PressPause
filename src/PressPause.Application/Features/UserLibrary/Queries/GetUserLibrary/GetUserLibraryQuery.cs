using MediatR;
using PressPause.Application.Common.Models;
using PressPause.Domain.Entities;

namespace PressPause.Application.Features.UserLibrary.Queries.GetUserLibrary;

public sealed record GetUserLibraryQuery(
    string UserId,
    int Page = 1,
    int PageSize = 20,
    GameStatus? Status = null)
    : IRequest<PagedResult<UserGameDto>>;