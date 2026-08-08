using FluentValidation;

namespace PressPause.Application.Developers.Commands.CreateDeveloper;

public sealed class CreateDeveloperCommandValidator
    : AbstractValidator<CreateDeveloperCommand>
{
    public CreateDeveloperCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}