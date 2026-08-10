namespace PressPause.Application.Games.Queries.GetGames;

public sealed record PlatformDto(
    Guid Id,
    string Name,
    string Slug);