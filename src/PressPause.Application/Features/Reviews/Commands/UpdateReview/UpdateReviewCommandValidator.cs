using FluentValidation;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed class UpdateReviewCommandValidator
    : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.ReviewId)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Text)
            .MaximumLength(2000);
    }
}