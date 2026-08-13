using PressPause.Application.Tags.Queries.GetTags;

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

    public IReadOnlyList<PublisherDto> Publishers { get; init; }
        = new List<PublisherDto>();

    public IReadOnlyList<StoreDto> Stores { get; init; }
        = new List<StoreDto>();

    public IReadOnlyCollection<TagDto> Tags { get; init; }
        = new List<TagDto>();
}