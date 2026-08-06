using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Platforms.Queries.GetPlatforms;

public sealed record GetPlatformsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<PlatformDto>>;