using FluentValidation;

namespace PressPause.Application.Platforms.Commands.UpdatePlatform;

public sealed class UpdatePlatformCommandValidator
    : AbstractValidator<UpdatePlatformCommand>
{
    public UpdatePlatformCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}