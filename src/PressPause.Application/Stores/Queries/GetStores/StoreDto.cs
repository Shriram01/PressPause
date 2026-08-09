namespace PressPause.Application.Stores.Queries.GetStores;

public sealed record StoreDto(
    Guid Id,
    string Name,
    string Slug);