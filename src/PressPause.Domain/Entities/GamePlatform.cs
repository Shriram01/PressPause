namespace PressPause.Domain.Entities;

public sealed class GamePlatform
{
    public Guid GameId { get; private set; }

    public Guid PlatformId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Platform Platform { get; private set; } = null!;

    private GamePlatform()
    {
    }

    public GamePlatform(
        Guid gameId,
        Guid platformId)
    {
        GameId = gameId;
        PlatformId = platformId;
    }
}