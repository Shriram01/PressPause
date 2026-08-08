using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Developers.Queries.GetDevelopers;

public sealed record GetDevelopersQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<DeveloperDto>>;