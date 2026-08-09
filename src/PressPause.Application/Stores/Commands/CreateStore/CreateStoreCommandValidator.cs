using FluentValidation;

namespace PressPause.Application.Stores.Commands.CreateStore;

public sealed class CreateStoreCommandValidator
    : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}