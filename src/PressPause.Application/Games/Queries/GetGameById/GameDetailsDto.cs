namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record GameDetailsDto(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate);