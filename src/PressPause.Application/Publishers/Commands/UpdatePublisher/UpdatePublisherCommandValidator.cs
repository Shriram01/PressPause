using FluentValidation;

namespace PressPause.Application.Publishers.Commands.UpdatePublisher;

public sealed class UpdatePublisherCommandValidator
    : AbstractValidator<UpdatePublisherCommand>
{
    public UpdatePublisherCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}