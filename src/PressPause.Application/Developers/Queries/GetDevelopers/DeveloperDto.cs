namespace PressPause.Application.Developers.Queries.GetDevelopers;

public sealed record DeveloperDto(
    Guid Id,
    string Name,
    string Slug);