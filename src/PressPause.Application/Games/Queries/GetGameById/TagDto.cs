namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug);