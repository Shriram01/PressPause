using MediatR;
using PressPause.Application.Authentication.Interfaces;

namespace PressPause.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, string?>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(
            request.Email,
            request.Password);
    }
}