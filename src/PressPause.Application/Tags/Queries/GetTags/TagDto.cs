namespace PressPause.Application.Tags.Queries.GetTags;

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug);