using FluentValidation;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed class AddUserGameCommandValidator
    : AbstractValidator<AddUserGameCommand>
{
    public AddUserGameCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.GameId)
            .NotEmpty()
            .WithMessage("Game ID is required.");
    }
}