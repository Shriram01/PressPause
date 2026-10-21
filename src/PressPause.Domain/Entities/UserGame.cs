using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class UserGame : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public Guid GameId { get; set; }

    public GameStatus Status { get; private set; } = GameStatus.PlanToPlay;

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public Game Game { get; set; } = null!;

    public void UpdateStatus(GameStatus status)
    {
        Status = status;

        if (status == GameStatus.Playing && StartedAt is null)
        {
            StartedAt = DateTime.UtcNow;
        }

        if (status == GameStatus.Completed)
        {
            CompletedAt = DateTime.UtcNow;
        }
        else
        {
            CompletedAt = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}

public enum GameStatus
{
    PlanToPlay = 1,
    Playing = 2,
    Completed = 3,
    Dropped = 4
}