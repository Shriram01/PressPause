using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Tags.Queries.GetTags;

public sealed record GetTagsQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<TagDto>>;