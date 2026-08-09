using FluentValidation;

namespace PressPause.Application.Stores.Commands.UpdateStore;

public sealed class UpdateTagCommandValidator
    : AbstractValidator<UpdateStoreCommand>
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