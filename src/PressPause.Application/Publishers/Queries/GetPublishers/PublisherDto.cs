namespace PressPause.Application.Publishers.Queries.GetPublishers;

public sealed record PublisherDto(
    Guid Id,
    string Name,
    string Slug);