using FluentValidation;

namespace PressPause.Application.Publishers.Commands.CreatePublisher;

public sealed class CreatePublisherCommandValidator
    : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100);
    }
}