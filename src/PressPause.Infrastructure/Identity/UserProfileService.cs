using Microsoft.AspNetCore.Identity;
using PressPause.Application.Authentication.Interfaces;

namespace PressPause.Infrastructure.Identity;

public sealed class UserProfileService : IUserProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileDto?> GetProfileAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.Bio,
            user.AvatarUrl);
    }

    public async Task<bool> UpdateProfileAsync(
        string userId,
        string displayName,
        string? bio,
        string? avatarUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return false;
        }

        user.DisplayName = displayName.Trim();
        user.Bio = bio?.Trim();
        user.AvatarUrl = avatarUrl?.Trim();

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }
}