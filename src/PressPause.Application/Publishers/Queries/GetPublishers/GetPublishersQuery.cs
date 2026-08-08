using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Publishers.Queries.GetPublishers;

public sealed record GetPublishersQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<PublisherDto>>;