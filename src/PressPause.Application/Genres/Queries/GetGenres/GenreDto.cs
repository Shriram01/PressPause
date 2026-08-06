namespace PressPause.Application.Genres.Queries.GetGenres;

public sealed record GenreDto(
    Guid Id,
    string Name,
    string Slug);