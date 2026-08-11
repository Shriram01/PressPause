namespace PressPause.Application.Games.Queries.GetGames;

public sealed record PublisherDto(
    Guid Id,
    string Name,
    string Slug);