namespace PressPause.Application.Games.Queries.GetGames;

public sealed record GenreDto(
    Guid Id,
    string Name,
    string Slug);