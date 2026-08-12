namespace PressPause.Domain.Entities;

public sealed class GameStore
{
    public Guid GameId { get; private set; }

    public Guid StoreId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Store Store { get; private set; } = null!;

    private GameStore()
    {
    }

    public GameStore(Guid gameId, Guid storeId)
    {
        GameId = gameId;
        StoreId = storeId;
    }
}