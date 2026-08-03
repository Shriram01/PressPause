namespace PressPause.Application.Authentication.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(
        string userId,
        string email,
        IEnumerable<string> roles);
}