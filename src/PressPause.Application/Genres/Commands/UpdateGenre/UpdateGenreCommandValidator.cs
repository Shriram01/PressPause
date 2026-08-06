using FluentValidation;

namespace PressPause.Application.Genres.Commands.UpdateGenre;

public sealed class UpdateGenreCommandValidator
    : AbstractValidator<UpdateGenreCommand>
{
    public UpdateGenreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}