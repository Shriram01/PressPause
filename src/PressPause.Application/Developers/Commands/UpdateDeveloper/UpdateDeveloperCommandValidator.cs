using FluentValidation;

namespace PressPause.Application.Developers.Commands.UpdateDeveloper;

public sealed class UpdateDeveloperCommandValidator
    : AbstractValidator<UpdateDeveloperCommand>
{
    public UpdateDeveloperCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}