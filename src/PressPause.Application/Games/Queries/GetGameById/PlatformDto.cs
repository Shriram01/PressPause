namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record PlatformDto(
    Guid Id,
    string Name,
    string Slug);