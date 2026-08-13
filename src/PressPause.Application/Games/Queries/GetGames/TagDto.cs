namespace PressPause.Application.Games.Queries.GetGames;

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug);