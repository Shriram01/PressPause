using PressPause.Domain.Common;

namespace PressPause.Domain.ValueObjects;

public sealed class GameSlug : ValueObject
{
    public string Value { get; }

    private GameSlug(string value)
    {
        Value = value;
    }

    public static GameSlug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Slug cannot be empty.");

        value = value.Trim().ToLowerInvariant();

        return new GameSlug(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(GameSlug slug) => slug.Value;
}