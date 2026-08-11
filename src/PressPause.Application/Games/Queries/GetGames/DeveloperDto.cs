namespace PressPause.Application.Games.Queries.GetGames;

public sealed record DeveloperDto(
    Guid Id,
    string Name,
    string Slug);