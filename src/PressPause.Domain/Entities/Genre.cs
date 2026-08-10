using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Genre : AuditableEntity
{
    public ICollection<GameGenre> GameGenres { get; private set; }
        = new List<GameGenre>();

    private Genre()
    {
    }

    public Genre(
        string name,
        string slug)
    {
        Name = name;
        Slug = slug;
    }

    public string Name { get; private set; } = default!;

    public string Slug { get; private set; } = default!;

    public void Update(
        string name,
        string slug)
    {
        Name = name;
        Slug = slug;
    }
}