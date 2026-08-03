using PressPause.Application.Common.Models;

namespace PressPause.Application.Authentication.Interfaces;

public interface IIdentityService
{
    Task<Result> RegisterAsync(
        string email,
        string password);

    Task<string?> LoginAsync(
        string email,
        string password);
}