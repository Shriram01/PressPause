using MediatR;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password) : IRequest<Result>;