namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record PublisherDto(
    Guid Id,
    string Name,
    string Slug);