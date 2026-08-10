namespace PressPause.Domain.Entities;

public sealed class GameGenre
{
    public Guid GameId { get; private set; }

    public Guid GenreId { get; private set; }

    public Game Game { get; private set; } = null!;

    public Genre Genre { get; private set; } = null!;

    private GameGenre()
    {
    }

    public GameGenre(
        Guid gameId,
        Guid genreId)
    {
        GameId = gameId;
        GenreId = genreId;
    }
}