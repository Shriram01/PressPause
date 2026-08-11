namespace PressPause.Application.Games.Queries.GetGames;

public sealed record GameDto(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate)
{
    public IReadOnlyList<PlatformDto> Platforms { get; init; }
        = new List<PlatformDto>();

    public IReadOnlyList<GenreDto> Genres { get; init; }
        = new List<GenreDto>();

    public IReadOnlyList<DeveloperDto> Developers { get; init; }
        = new List<DeveloperDto>();
    
    public IReadOnlyList<PublisherDto> Publishers { get; init; }
        = new List<PublisherDto>();
    
}