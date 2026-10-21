using Microsoft.AspNetCore.Identity;

namespace PressPause.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
}