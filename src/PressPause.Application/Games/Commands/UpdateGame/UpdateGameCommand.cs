using MediatR;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed record UpdateGameCommand(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate) : IRequest
{
    public IReadOnlyList<Guid> PlatformIds { get; init; }
        = new List<Guid>();

    public IReadOnlyList<Guid> GenreIds { get; init; }
        = new List<Guid>();

    public IReadOnlyList<Guid> DeveloperIds { get; init; }
        = new List<Guid>();
    
    public IReadOnlyList<Guid> PublisherIds { get; init; }
        = new List<Guid>();
}