using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Stores.Queries.GetStores;

namespace PressPause.Application.Stores.Queries.GetStoreById;

public sealed class GetStoreByIdQueryHandler
    : IRequestHandler<GetStoreByIdQuery, StoreDto?>
{
    private readonly IStoreRepository _repository;

    public GetStoreByIdQueryHandler(
        IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<StoreDto?> Handle(
        GetStoreByIdQuery request,
        CancellationToken cancellationToken)
    {
        var store = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (store is null)
        {
            return null;
        }

        return new StoreDto(
            store.Id,
            store.Name,
            store.Slug);
    }
}