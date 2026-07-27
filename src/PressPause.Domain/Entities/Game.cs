using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Game : AuditableEntity
{
    public void Update(
        string title,
        string slug,
        DateOnly? releaseDate)
    {
        Title = title;
        Slug = slug;
        ReleaseDate = releaseDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public DateOnly? ReleaseDate { get; set; }
}