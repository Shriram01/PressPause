using MediatR;
using PressPause.Application.Stores.Queries.GetStores;

namespace PressPause.Application.Stores.Queries.GetStoreById;

public sealed record GetStoreByIdQuery(Guid Id)
    : IRequest<StoreDto?>;