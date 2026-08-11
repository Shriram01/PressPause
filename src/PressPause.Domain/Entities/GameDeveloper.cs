namespace PressPause.Domain.Entities;

public sealed class GameDeveloper
{
    public Guid GameId { get; private set; }

    public Guid DeveloperId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Developer Developer { get; private set; } = null!;

    private GameDeveloper()
    {
    }

    public GameDeveloper(
        Guid gameId,
        Guid developerId)
    {
        GameId = gameId;
        DeveloperId = developerId;
    }
}