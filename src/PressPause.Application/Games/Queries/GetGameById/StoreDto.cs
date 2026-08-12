namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record StoreDto(
    Guid Id,
    string Name,
    string Slug);