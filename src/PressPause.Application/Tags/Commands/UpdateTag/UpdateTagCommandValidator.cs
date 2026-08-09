using FluentValidation;

namespace PressPause.Application.Tags.Commands.UpdateTag;

public sealed class UpdateTagCommandValidator
    : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}