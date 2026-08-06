using FluentValidation;

namespace PressPause.Application.Platforms.Commands.CreatePlatform;

public sealed class CreatePlatformCommandValidator
    : AbstractValidator<CreatePlatformCommand>
{
    public CreatePlatformCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}