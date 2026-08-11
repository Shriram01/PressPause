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

    public ICollection<GamePlatform> GamePlatforms { get; private set; }
        = new List<GamePlatform>();

    public ICollection<GameGenre> GameGenres { get; private set; }
        = new List<GameGenre>();

    public ICollection<GameDeveloper> GameDevelopers { get; private set; }
        = new List<GameDeveloper>();

    public ICollection<GamePublisher> GamePublishers { get; private set; }
        = new List<GamePublisher>();
}