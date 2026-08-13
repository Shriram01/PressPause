namespace PressPause.Domain.Entities;

public sealed class GameTag
{
    public Guid GameId { get; private set; }

    public Guid TagId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Tag Tag { get; private set; } = null!;

    private GameTag()
    {
    }

    public GameTag(
        Guid gameId,
        Guid tagId)
    {
        GameId = gameId;
        TagId = tagId;
    }
}