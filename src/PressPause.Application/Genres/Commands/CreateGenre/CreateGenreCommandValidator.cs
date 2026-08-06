using FluentValidation;

namespace PressPause.Application.Genres.Commands.CreateGenre;

public sealed class CreateGenreCommandValidator
    : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}