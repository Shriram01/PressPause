namespace PressPause.Application.Games.Queries.GetGames;

public sealed record GameDto(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate)
{
    public IReadOnlyList<PlatformDto> Platforms { get; init; } = [];
}