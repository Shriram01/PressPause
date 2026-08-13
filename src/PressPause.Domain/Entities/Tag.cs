using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Tag : AuditableEntity
{
    private Tag()
    {
    }

    public Tag(
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

    public ICollection<GameTag> GameTags { get; private set; }
        = new List<GameTag>();
}