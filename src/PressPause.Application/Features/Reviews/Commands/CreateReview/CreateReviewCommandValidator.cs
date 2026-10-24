using FluentValidation;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed class CreateReviewCommandValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.GameId)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Text)
            .MaximumLength(2000);
    }
}