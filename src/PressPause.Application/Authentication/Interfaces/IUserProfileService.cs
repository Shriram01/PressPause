namespace PressPause.Application.Authentication.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetProfileAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateProfileAsync(
        string userId,
        string displayName,
        string? bio,
        string? avatarUrl,
        CancellationToken cancellationToken = default);
}

public sealed record UserProfileDto(
    string Id,
    string? Email,
    string DisplayName,
    string? Bio,
    string? AvatarUrl);