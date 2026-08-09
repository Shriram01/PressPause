using FluentValidation;

namespace PressPause.Application.Stores.Commands.UpdateStore;

public sealed class UpdateStoreCommandValidator
    : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}