using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Stores.Queries.GetStores;

public sealed class GetStoresQueryHandler
    : IRequestHandler<GetStoresQuery, PagedResult<StoreDto>>
{
    private readonly IStoreRepository _repository;

    public GetStoresQueryHandler(
        IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<StoreDto>> Handle(
        GetStoresQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.Descending,
            cancellationToken);

        var dto = items
            .Select(x => new StoreDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<StoreDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}