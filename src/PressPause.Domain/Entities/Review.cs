using PressPause.Domain.Common;

namespace PressPause.Domain.Entities;

public sealed class Review : AuditableEntity
{
    private Review()
    {
    }

    public Review(
        string userId,
        Guid gameId,
        int rating,
        string? text)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User ID is required.", nameof(userId));

        if (gameId == Guid.Empty)
            throw new ArgumentException(
                "Game ID is required.", nameof(gameId));

        ValidateRating(rating);

        UserId = userId;
        GameId = gameId;
        Rating = rating;
        Text = NormalizeText(text);
    }

    public string UserId { get; private set; } = string.Empty;

    public Guid GameId { get; private set; }

    public int Rating { get; private set; }

    public string? Text { get; private set; }

    public Game Game { get; private set; } = null!;

    public void Update(int rating, string? text)
    {
        ValidateRating(rating);

        Rating = rating;
        Text = NormalizeText(text);
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRating(int rating)
    {
        if (rating is < 1 or > 10)
            throw new ArgumentOutOfRangeException(
                nameof(rating),
                "Rating must be between 1 and 10.");
    }

    private static string? NormalizeText(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }
}