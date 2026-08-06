namespace PressPause.Application.Platforms.Queries.GetPlatforms;

public sealed record PlatformDto(
    Guid Id,
    string Name,
    string Slug);