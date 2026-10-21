using Microsoft.AspNetCore.Identity;
using PressPause.Application.Authentication.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result> RegisterAsync(
        string email,
        string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return Result.Failure("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return Result.Failure(
                createResult.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return Result.Failure(
                roleResult.Errors.Select(e => e.Description));
        }

        return Result.Success();
    }

    public async Task<string?> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return await _jwtTokenGenerator.GenerateTokenAsync(
            user.Id,
            user.Email!,
            roles);
    }
}