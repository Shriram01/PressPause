namespace PressPause.Application.Games.Queries.GetGameById;

public sealed record GameDetailsDto(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate)
{
    public IReadOnlyList<PlatformDto> Platforms { get; init; } = [];

    public IReadOnlyList<GenreDto> Genres { get; init; }
        = new List<GenreDto>();

    public IReadOnlyList<DeveloperDto> Developers { get; init; }
        = new List<DeveloperDto>();
}