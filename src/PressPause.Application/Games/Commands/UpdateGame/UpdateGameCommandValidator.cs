using FluentValidation;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed class UpdateGameCommandValidator
    : AbstractValidator<UpdateGameCommand>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(200);
    }
}