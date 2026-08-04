using MediatR;

namespace PressPause.Application.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<string?>;