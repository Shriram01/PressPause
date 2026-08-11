using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Publisher : AuditableEntity
{
    private Publisher()
    {
    }

    public Publisher(
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

    public ICollection<GamePublisher> GamePublishers { get; private set; }
        = new List<GamePublisher>();
}