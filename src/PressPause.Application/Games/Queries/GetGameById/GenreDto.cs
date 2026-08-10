namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record GenreDto(
    Guid Id,
    string Name,
    string Slug);