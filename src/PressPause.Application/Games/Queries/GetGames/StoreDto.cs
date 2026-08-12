namespace PressPause.Application.Games.Queries.GetGames;

public sealed record StoreDto(
    Guid Id,
    string Name,
    string Slug);