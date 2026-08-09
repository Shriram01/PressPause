using FluentValidation;

namespace PressPause.Application.Tags.Commands.CreateTag;

public sealed class CreateTagCommandValidator
    : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}