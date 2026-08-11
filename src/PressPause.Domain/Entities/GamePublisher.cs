namespace PressPause.Domain.Entities;

public sealed class GamePublisher
{
    public Guid GameId { get; private set; }

    public Guid PublisherId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Publisher Publisher { get; private set; } = null!;

    private GamePublisher()
    {
    }

    public GamePublisher(
        Guid gameId,
        Guid publisherId)
    {
        GameId = gameId;
        PublisherId = publisherId;
    }
}