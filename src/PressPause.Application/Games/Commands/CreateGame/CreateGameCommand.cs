using MediatR;

namespace PressPause.Application.Games.Commands.CreateGame;

public sealed record CreateGameCommand(
    string Title,
    string Slug,
    DateOnly? ReleaseDate) : IRequest<Guid>
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