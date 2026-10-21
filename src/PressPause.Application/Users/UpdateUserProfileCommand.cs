using FluentValidation;
using MediatR;
using PressPause.Application.Authentication.Interfaces;

namespace PressPause.Application.Users;

public sealed record UpdateUserProfileCommand(
    string UserId,
    string DisplayName,
    string? Bio,
    string? AvatarUrl) : IRequest<bool>;

public sealed class UpdateUserProfileCommandValidator
    : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Bio)
            .MaximumLength(500);

        RuleFor(x => x.AvatarUrl)
            .Must(url =>
                string.IsNullOrWhiteSpace(url) ||
                Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttps ||
                 uri.Scheme == Uri.UriSchemeHttp))
            .WithMessage("Avatar URL must be a valid HTTP or HTTPS URL.");
    }
}

public sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserProfileService _profileService;

    public UpdateUserProfileCommandHandler(
        IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<bool> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        return _profileService.UpdateProfileAsync(
            request.UserId,
            request.DisplayName,
            request.Bio,
            request.AvatarUrl,
            cancellationToken);
    }
}