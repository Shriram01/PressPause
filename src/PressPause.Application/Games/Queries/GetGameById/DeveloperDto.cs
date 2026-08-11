namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record DeveloperDto(
    Guid Id,
    string Name,
    string Slug);