using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Genres.Queries.GetGenres;

public sealed record GetGenresQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<GenreDto>>;