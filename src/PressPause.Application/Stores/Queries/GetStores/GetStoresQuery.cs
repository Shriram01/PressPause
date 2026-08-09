using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Stores.Queries.GetStores;

public sealed record GetStoresQuery(
    int Page,
    int PageSize,
    string? Search,
    string? SortBy,
    bool Descending)
    : IRequest<PagedResult<StoreDto>>;