using FluentValidation;

namespace PressPause.Application.Features.Reviews.Queries;

public sealed class GetGameReviewsQueryValidator
    : AbstractValidator<GetGameReviewsQuery>
{
    public GetGameReviewsQueryValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}