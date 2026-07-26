using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Game : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public DateOnly? ReleaseDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}